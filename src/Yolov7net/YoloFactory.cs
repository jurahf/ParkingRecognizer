using IVilson.AI.Yolov7net.YoloVersions;
using Yolov7net;

namespace IVilson.AI.Yolov7net
{
    public class YoloFactory : IYoloFactory
    {
        public IYoloNet CreateYolo(YoloVersion yoloVersion)
        {
            switch (yoloVersion)
            {
                case YoloVersion.Yolo5:
                    return new Yolov5("./assets/yolov5-tiny_640x640.onnx", true);
                case YoloVersion.Yolo7tiny:
                    return new Yolov7tiny("./assets/yolov7-tiny.onnx", false);
                case YoloVersion.Yolo7:
                    return new Yolov7("./assets/yolov7_post_736x1280.onnx", false);
                case YoloVersion.Yolo8:
                    return new Yolov8("./assets/yolov8n.onnx", true);
                case YoloVersion.Yolo9:
                    return new Yolov9("./assets/yolov9-c.onnx", true);
                case YoloVersion.Yolo10:
                    return new Yolov10("./assets/yolov10n.onnx", false);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
