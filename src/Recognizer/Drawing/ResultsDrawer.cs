using SkiaSharp;

namespace Recognizer.Drawing
{
    public class ResultsDrawer : IResultsDrawer
    {
        public void DrawRectangles(SKBitmap image, List<SKRect> rects, SKColor color)
        {
            var rectPaint = GetRectPaint(color);
            var textPaint = GetTextPaint(color);

            using (var canvas = new SKCanvas(image))
            {
                int i = 0;
                foreach (var rect in rects)
                {
                    canvas.DrawRect(rect, rectPaint);

                    var x = rect.Left + 3;
                    var y = rect.Top + 23;
                    canvas.DrawText($"{i++}", x, y, textPaint);
                }

                canvas.Flush();
            }
        }


        private SKPaint GetRectPaint(SKColor color)
        {
            return new SKPaint()
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 5,
                IsAntialias = true,
                Color = color
            };
        }

        private SKPaint GetTextPaint(SKColor color)
        {
            return new SKPaint
            {
                TextSize = 16,
                IsAntialias = true,
                Color = color,
                IsStroke = false,
                StrokeWidth = 5
            };
        }

        /// <summary>
        /// Сохраняет нарисованное изображение в файл
        /// </summary>
        /// <param name="image"></param>
        /// <param name="path"></param>
        public void SaveImage(SKBitmap image, string path)
        {
            using (var imageStream = new SKFileWStream(path))
            {
                image.Encode(imageStream, SKEncodedImageFormat.Jpeg, 100);
            }
        }

    }
}
