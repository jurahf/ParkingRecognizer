using ParkingPlaces.Models;

namespace Recognizer.Recognizing
{
    public class CameraRecognationResult
    {
        public Camera Camera { get; set; }

        public List<ViewedPlace> EmptyPlaces { get; set; }

        public List<ViewedPlace> FilledPlaces { get; set; }

        public string ResultImagePath { get; set; }

        public CameraRecognationResult()
        {
            EmptyPlaces = new List<ViewedPlace>();
            FilledPlaces = new List<ViewedPlace>();
        }
    }
}
