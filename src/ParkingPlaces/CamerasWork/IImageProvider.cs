using ParkingPlaces.Models;

namespace ParkingPlaces.CamerasWork
{
    public interface IImageProvider
    {
        string GetImagePath(Camera camera, out bool isOnline);
    }
}
