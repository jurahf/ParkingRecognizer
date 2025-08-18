using Microsoft.ML.OnnxRuntime.Tensors;
using SkiaSharp;

namespace Yolov7net.Helpers
{
    public static class Utils
    {
        /// <summary>
        /// Делаем тензор из картинки
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Tensor<float> GetTensorForSKImage(SKBitmap image)
        {
            var bytes = image.GetPixelSpan();
            var expectedOutputLength = image.Width * image.Height * 3;
            float[] channelData = new float[expectedOutputLength];

            var expectedChannelLength = expectedOutputLength / 3;
            var greenOffset = expectedChannelLength;
            var blueOffset = expectedChannelLength * 2;

            for (int i = 0, i2 = 0; i < bytes.Length; i += 4, i2++)
            {
                var b = Convert.ToSingle(bytes[i]);
                var g = Convert.ToSingle(bytes[i + 1]);
                var r = Convert.ToSingle(bytes[i + 2]);
                channelData[i2] = (r) / 255.0f;
                channelData[i2 + greenOffset] = (g) / 255.0f;
                channelData[i2 + blueOffset] = (b) / 255.0f;
            }

            return new DenseTensor<float>(new Memory<float>(channelData), new[] { 1, 3, image.Height, image.Width });
        }

        public static SKBitmap ResizeImage(SKBitmap image, int targetWidth, int targetHeight)
        {
            var resized = new SKBitmap(targetWidth, targetHeight, image.ColorType, image.AlphaType);
            using (var canvas = new SKCanvas(resized))
            {
                canvas.Clear(SKColors.Transparent);
                var paint = new SKPaint
                {
                    FilterQuality = SKFilterQuality.High,
                    IsAntialias = true
                };
                canvas.DrawBitmap(image, SKRect.Create(0, 0, targetWidth, targetHeight), paint);
            }
            return resized;
        }

    }
}
