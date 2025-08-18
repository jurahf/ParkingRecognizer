using ParkingPlaces.Models;

namespace Recognizer.Recognizing
{
    public class ParkingRecognationResult
    {
        public List<Place> EmptyPlaces { get; set; }

        public List<Place> FilledPlaces { get; set; }

        public List<Place> InQuestionPlaces { get; set; }

        public List<string> ImagePaths { get; set; }

        public ParkingRecognationResult()
        {
            EmptyPlaces = new List<Place>();
            FilledPlaces = new List<Place>();
            InQuestionPlaces = new List<Place>();
            ImagePaths = new List<string>();
        }
    }
}
