using IVilson.AI.Yolov7net.Models;
using SkiaSharp;

namespace Yolov7net
{
    public interface IYoloNet : IDisposable
    {
        public void SetupLabels(string[] labels);
        public void SetupYoloDefaultLabels();
        public List<YoloPrediction> Predict(SKBitmap image, float conf_thres = 0, float iou_thres = 0);
    }
}
