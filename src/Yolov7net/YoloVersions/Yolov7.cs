using IVilson.AI.Yolov7net.Models;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SkiaSharp;
using Yolov7net;
using Yolov7net.Helpers;
using Yolov7net.Models;


namespace IVilson.AI.Yolov7net.YoloVersions
{
    public class Yolov7 : IYoloNet
    {
        private readonly InferenceSession _inferenceSession;
        private readonly YoloModel _model = new YoloModel();

        public Yolov7(string modelPath, bool useCuda = false)
        {
            if (useCuda)
            {
                SessionOptions opts = SessionOptions.MakeSessionOptionWithCudaProvider();
                _inferenceSession = new InferenceSession(modelPath, opts);
            }
            else
            {
                SessionOptions opts = new();
                _inferenceSession = new InferenceSession(modelPath, opts);
            }

            // Get model info
            get_input_details();
            get_output_details();
        }

        public void SetupLabels(string[] labels)
        {
            labels.Select((s, i) => new { i, s }).ToList().ForEach(item =>
            {
                _model.Labels.Add(new YoloLabel { Id = item.i, Name = item.s });
            });
        }

        public void SetupYoloDefaultLabels()
        {
            var s = new string[] { "person", "bicycle", "car", "motorcycle", "airplane", "bus", "train", "truck", "boat", "traffic light", "fire hydrant", "stop sign", "parking meter", "bench", "bird", "cat", "dog", "horse", "sheep", "cow", "elephant", "bear", "zebra", "giraffe", "backpack", "umbrella", "handbag", "tie", "suitcase", "frisbee", "skis", "snowboard", "sports ball", "kite", "baseball bat", "baseball glove", "skateboard", "surfboard", "tennis racket", "bottle", "wine glass", "cup", "fork", "knife", "spoon", "bowl", "banana", "apple", "sandwich", "orange", "broccoli", "carrot", "hot dog", "pizza", "donut", "cake", "chair", "couch", "potted plant", "bed", "dining table", "toilet", "tv", "laptop", "mouse", "remote", "keyboard", "cell phone", "microwave", "oven", "toaster", "sink", "refrigerator", "book", "clock", "vase", "scissors", "teddy bear", "hair drier", "toothbrush" };
            SetupLabels(s);
        }

        public List<YoloPrediction> Predict(SKBitmap image, float conf_thres = 0, float iou_thres = 0)
        {
            using var outputs = Inference(image);
            string scoresOutputName = _model.Outputs[0];
            string batchClassX1y1x2y2OutputName = _model.Outputs[1];

            var scoresOutput = (DenseTensor<float>)outputs.First(x => x.Name == scoresOutputName).Value;
            var batchClassX1y1x2y2Output = (DenseTensor<long>)outputs.First(x => x.Name == batchClassX1y1x2y2OutputName).Value;
            return ParseDetect(scoresOutput, batchClassX1y1x2y2Output, image);
        }

        private List<YoloPrediction> ParseDetect(DenseTensor<float> scoresOutput, DenseTensor<long> batchClassX1y1x2y2Output, SKBitmap image)
        {
            var predictions = new List<YoloPrediction>();

            var (w, h) = (image.Width, image.Height);
            var (xGain, yGain) = (_model.Width / (float)w, _model.Height / (float)h);
            var gain = Math.Min(xGain, yGain);
            var (xPad, yPad) = ((_model.Width - w * gain) / 2, (_model.Height - h * gain) / 2);

            for (int i = 0; i < batchClassX1y1x2y2Output.Dimensions[0]; i++)
            {
                var span = batchClassX1y1x2y2Output.Buffer.Span.Slice(i * batchClassX1y1x2y2Output.Strides[0]);
                var label = _model.Labels[(int)span[1]];
                var score = scoresOutput[i];

                if (score < _model.Confidence)
                    continue;  // Skip detections below confidence threshold

                var xMin = span[3] / xGain;
                var yMin = span[2] / yGain;
                var xMax = span[5] / xGain;
                var yMax = span[4] / yGain;

                var prediction = new YoloPrediction
                {
                    Label = label,
                    Score = score,
                    Rectangle = new SKRect(xMin, yMin, xMax, yMax)
                };

                predictions.Add(prediction);
            }

            return predictions;
        }


        private IDisposableReadOnlyCollection<DisposableNamedOnnxValue> Inference(SKBitmap img)
        {
            SKBitmap resized;

            if (img.Width != _model.Width || img.Height != _model.Height)
            {
                resized = Utils.ResizeImage(img, _model.Width, _model.Height); // fit image size to specified input size
            }
            else
            {
                resized = img;
            }

            var inputs = new[] // add image as onnx input
            {
                NamedOnnxValue.CreateFromTensor("images", Utils.GetTensorForSKImage(resized))
            };

            return _inferenceSession.Run(inputs, _model.Outputs); // run inference
        }

        private void get_input_details()
        {
            _model.Height = _inferenceSession.InputMetadata["images"].Dimensions[2];
            _model.Width = _inferenceSession.InputMetadata["images"].Dimensions[3];
        }

        private void get_output_details()
        {
            _model.Outputs = _inferenceSession.OutputMetadata.Keys.ToArray();
            _model.Dimensions = _inferenceSession.OutputMetadata[_model.Outputs[0]].Dimensions[1];
            _model.UseDetect = !_model.Outputs.Any(x => x == "score");
        }

        public void Dispose()
        {
            _inferenceSession.Dispose();
        }

    }
}
