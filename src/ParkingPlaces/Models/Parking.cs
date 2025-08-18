namespace ParkingPlaces.Models
{
    public class Parking
    {
        public int Id { get; set; }

        public List<Camera> Cameras { get; set; }

        public List<Place> Places { get; set; }


        public Parking()
        {
            Cameras = new List<Camera>();
            Places = new List<Place>();
        }
    }
}
