namespace ParkingPlaces.Models
{
    /// <summary>
    /// Камера наблюдения на парковке
    /// </summary>
    public class Camera
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string RtspUrl { get; set; }

        public int ParkingId { get; set; }

        public List<ViewedPlace> ViewedPlaces { get; set; }

        public Camera()
        {
            ViewedPlaces = new List<ViewedPlace>();
        }
    }
}
