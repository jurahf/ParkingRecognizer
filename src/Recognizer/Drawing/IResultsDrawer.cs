using SkiaSharp;

namespace Recognizer.Drawing
{
    public interface IResultsDrawer
    {
        void DrawRectangles(SKBitmap image, List<SKRect> rects, SKColor color);

        void SaveImage(SKBitmap image, string path);
    }
}
