using Newtonsoft.Json;
using SkiaSharp;

namespace ParkingPlaces.Models
{
    /// <summary>
    /// Парковочное место, просматриваемое с камеры (одно физическое место может быть видно с нескольких камер)
    /// </summary>
    public class ViewedPlace
    {
        public int Id { get; set; }

        /// <summary>
        /// Id камеры
        /// </summary>
        public int CameraId { get; set; }

        /// <summary>
        /// Id физического места
        /// </summary>
        public int PlaceId { get; set; }

        public Place Place { get; set; }

        /// <summary>
        /// Левый край
        /// </summary>
        public float X1 { get; set; }

        /// <summary>
        /// Верхний край
        /// </summary>
        public float Y1 { get; set; }

        /// <summary>
        /// Правый край
        /// </summary>
        public float X2 { get; set; }

        /// <summary>
        /// Нижний край
        /// </summary>
        public float Y2 { get; set; }

        /// <summary>
        /// Прямоугольник, ассоциированный с местом на снимке с камеры
        /// </summary>
        [JsonIgnore]
        public SKRect Rectangle => new SKRect(X1, Y1, X2, Y2);
    }
}
