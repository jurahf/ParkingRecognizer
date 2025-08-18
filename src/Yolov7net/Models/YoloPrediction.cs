using SkiaSharp;

namespace IVilson.AI.Yolov7net.Models
{
    /// <summary>
    /// Результат распознавания
    /// </summary>
    public class YoloPrediction
    {
        /// <summary>
        /// Тип распознанного объекта
        /// </summary>
        public YoloLabel? Label { get; set; }

        /// <summary>
        /// Расположение распознанного объекта (в координатах исходной картинки)
        /// </summary>
        public SKRect Rectangle { get; set; }

        /// <summary>
        /// Уверенность распознавания
        /// </summary>
        public float Score { get; set; }

        public YoloPrediction() { }

        public YoloPrediction(YoloLabel label, float confidence) : this(label)
        {
            Score = confidence;
        }

        public YoloPrediction(YoloLabel label)
        {
            Label = label;
        }
    }
}
