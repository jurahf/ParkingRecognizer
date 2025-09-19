using IVilson.AI.Yolov7net;
using IVilson.AI.Yolov7net.Models;
using IVilson.AI.Yolov7net.YoloVersions;
using Microsoft.Extensions.Caching.Memory;
using ParkingPlaces.CamerasWork;
using ParkingPlaces.Models;
using Recognizer.Drawing;
using SkiaSharp;
using Yolov7net;

namespace Recognizer.Recognizing
{
    public class ParkingRecognizer : IParkingRecognizer
    {
        /// <summary>
        /// Процент перекрытия, начиная с которого считаем, что место занято
        /// </summary>
        private const float minOverlap = 0.45f;

        private static object lockObj = new();
        private readonly IMemoryCache memoryCache;
        private readonly TimeSpan cacheExpiration = TimeSpan.FromSeconds(60);
        private readonly IImageProvider imageProvider;
        private readonly IYoloFactory factory;
        private readonly IResultsDrawer drawer;
        private readonly string[] CarsNames = new[] { "car", "motorcycle", "airplane", "bus", "train", "truck", "boat" };

        public ParkingRecognizer(IImageProvider imageProvider, IYoloFactory factory, IResultsDrawer drawer, IMemoryCache memoryCache)
        {
            this.imageProvider = imageProvider;
            this.factory = factory;
            this.drawer = drawer;
            this.memoryCache = memoryCache;
        }

        public ParkingRecognationResult RecognizeAll(Parking parking)
        {
            lock (lockObj)
            {
                if (memoryCache.TryGetValue(nameof(ParkingRecognationResult), out ParkingRecognationResult cachedResult) && cachedResult != null)
                {
                    return cachedResult;
                }
                else
                {
                    var resultList = new List<CameraRecognationResult>();

                    using (var yolo = factory.CreateYolo(YoloVersion.Yolo7))
                    {
                        yolo.SetupYoloDefaultLabels();

                        foreach (var camera in parking.Cameras)
                        {
                            var cameraResult = GetCameraRecognationResult(camera, yolo);

                            if (cameraResult != null)
                                resultList.Add(cameraResult);
                        }
                    }

                    var result = Merge(resultList);
                    memoryCache.Set(nameof(ParkingRecognationResult), result, cacheExpiration);

                    return result;
                }
            }
        }

        private CameraRecognationResult? GetCameraRecognationResult(Camera camera, IYoloNet yolo)
        {
            string sourceImage = imageProvider.GetImagePath(camera, out bool isOnline);
            if (!isOnline)
            {
                return null;
            }

            // TODO: Result image Path. Сейчас попадет в ту же папку, куда сохраняет камера
            string resultImage = Path.Combine(
                Path.GetDirectoryName(sourceImage) ?? "",
                $"{Path.GetFileNameWithoutExtension(sourceImage)}-recognation{Path.GetExtension(sourceImage)}");
            CameraRecognationResult cameraResult = new CameraRecognationResult()
            {
                Camera = camera,
                ResultImagePath = resultImage,
            };

            using (var image = SKBitmap.Decode(sourceImage))
            {
                var predictions = yolo.Predict(image);
                predictions = predictions.Where(x => CarsNames.Contains(x.Label?.Name)).ToList();

                /////////////////////////////////////////////////////////////// ДЛЯ ОТЛАДКИ - рисуем распознанное
                drawer.DrawRectangles(image, predictions.Select(x => x.Rectangle).ToList(), SKColors.Red);
                ///////////////////////////////////////////////////////////////

                SetFillAndEmptyPlaces(cameraResult, predictions);

                // TODO: рисуем, но не факт, что тут место для этого
                drawer.DrawRectangles(image, cameraResult.EmptyPlaces.Select(x => x.Rectangle).ToList(), SKColors.Green);
                drawer.DrawRectangles(image, cameraResult.FilledPlaces.Select(x => x.Rectangle).ToList(), SKColors.Red);
                drawer.SaveImage(image, resultImage);

                return cameraResult;
            }
        }

        private void SetFillAndEmptyPlaces(CameraRecognationResult cameraResult, List<YoloPrediction> predictions)
        {
            List<ViewedPlace> viewedPlaces = cameraResult.Camera.ViewedPlaces;

            foreach (var place in viewedPlaces)
            {
                // проверяем, пересекается ли с распознанными квадратами
                var intersect = predictions.Select(x =>
                    new
                    {
                        IntersectArea = Area(SKRect.Intersect(place.Rectangle, x.Rectangle)),
                        Prediction = x
                    })
                    .OrderByDescending(x => x.IntersectArea)
                    .FirstOrDefault();

                float overlap = 0f;
                if (intersect != null)
                {
                    var prediction = intersect.Prediction;
                    float intersectArea = intersect.IntersectArea;
                    float unionArea = Area(place.Rectangle) + Area(prediction.Rectangle) - intersectArea;
                    overlap = intersectArea / unionArea; // процент перекрытия
                }

                if (overlap >= minOverlap)
                {
                    cameraResult.FilledPlaces.Add(place);
                }
                else
                {
                    cameraResult.EmptyPlaces.Add(place);
                }
            }
        }

        private float Area(SKRect rect)
        {
            return rect.Width * rect.Height;
        }


        private ParkingRecognationResult Merge(List<CameraRecognationResult> cameraResults)
        {
            var result = new ParkingRecognationResult()
            {
                ImagePaths = cameraResults.Select(x => x.ResultImagePath).ToList(),
                EmptyPlaces = cameraResults.SelectMany(x => x.EmptyPlaces.Select(x => x.Place)).ToList(),
                FilledPlaces = cameraResults.SelectMany(x => x.FilledPlaces.Select(x => x.Place)).ToList(),
            };

            foreach (var place in result.EmptyPlaces)
            {
                if (result.FilledPlaces.Any(x => x.Id == place.Id))
                {
                    result.EmptyPlaces = result.EmptyPlaces.Where(x => x.Id != place.Id).ToList();
                    result.FilledPlaces = result.FilledPlaces.Where(x => x.Id != place.Id).ToList();
                    result.InQuestionPlaces.Add(place);
                }
            }

            return result;
        }


    }
}
