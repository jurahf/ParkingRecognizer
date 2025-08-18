using IVilson.AI.Yolov7net.YoloVersions;
using Yolov7net;

namespace IVilson.AI.Yolov7net
{
    public interface IYoloFactory
    {
        IYoloNet CreateYolo(YoloVersion yoloVersion);
    }
}
