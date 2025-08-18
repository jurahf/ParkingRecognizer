namespace IVilson.AI.Yolov7net.YoloVersions
{
    public class Yolov9 : Yolov8
    {
        public Yolov9(string modelPath, bool useCuda = false) : base(modelPath, useCuda)
        {
        }
    }
}
