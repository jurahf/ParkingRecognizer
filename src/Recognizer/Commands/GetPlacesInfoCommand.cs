using ParkingPlaces.Models;
using ParkingPlaces.Repositories;
using Recognizer.Recognizing;

namespace Recognizer.Commands
{
    public class GetPlacesInfoCommand
    {
        private readonly IRepository<Parking> repository;
        private readonly IParkingRecognizer recognizer;

        public GetPlacesInfoCommand(IRepository<Parking> repository, IParkingRecognizer recognizer)
        {
            this.repository = repository;
            this.recognizer = recognizer;
        }

        public ParkingRecognationResult Execute()
        {
            // TODO: 
            Parking parking = repository.GetById(1);

            var result = recognizer.RecognizeAll(parking);

            return result;
        }
    }
}
