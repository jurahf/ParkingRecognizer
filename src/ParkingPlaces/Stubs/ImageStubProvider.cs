using ParkingPlaces.CamerasWork;
using ParkingPlaces.Models;

namespace ParkingPlaces.Stubs
{
    public class ImageStubProvider : IImageProvider
    {
        public string GetImagePath(Camera camera, out bool isOnline)
        {
            isOnline = true;
            return $"Stubs\\Assets\\photo_day_{camera.Id}.jpg";

            //return "Assets\\parking5.jpg";
        }
    }
}
