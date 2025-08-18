using IVilson.AI.Yolov7net.Models;

namespace Yolov7net.Models
{
    public class YoloModel
    {
        /// <summary>
        /// Ширина входной картинки
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Высота входной картинки
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Количество измерений выходного тензора
        /// </summary>
        public int Dimensions { get; set; }

        /// <summary>
        /// Порог уверенности распознавания
        /// </summary>
        public float Confidence { get; set; } = 0.20f;

        /// <summary>
        /// Для версии 5 - дополнительный порог уверенности
        /// </summary>
        public float MulConfidence { get; set; } = 0.25f;

        /// <summary>
        /// В версиях 5 и 8. Процент пересечения распознанных объектов, свыше которого один из них отбрасывается
        /// </summary>
        public float Overlap { get; set; } = 0.45f;

        /// <summary>
        /// Названия выходных колонок
        /// </summary>
        public string[] Outputs { get; set; }

        /// <summary>
        /// Список типов объектов, которые может распознать нейросеть
        /// </summary>
        public List<YoloLabel> Labels { get; set; } = new List<YoloLabel>();

        /// <summary>
        /// Используется ли отдельная выходная колонка для уверенности распознавания
        /// </summary>
        public bool UseDetect { get; set; }
    }
}
