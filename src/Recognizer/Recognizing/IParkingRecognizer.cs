using ParkingPlaces.Models;

namespace Recognizer.Recognizing
{
    public interface IParkingRecognizer
    {
        ParkingRecognationResult RecognizeAll(Parking parking);
    }
}
