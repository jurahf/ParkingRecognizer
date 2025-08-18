using Newtonsoft.Json;
using ParkingPlaces.Models;
using Serilog;

namespace ParkingPlaces.Repositories;

public class ParkingJsonRepository : IRepository<Parking>
{
    private const string jsonPath = "Repositories/parking.json";
    private List<Parking> content = new List<Parking>();


    public ParkingJsonRepository()
    {
        try
        {
            Log.Logger.Information($"Загружаем данные о парковках");

            using StreamReader reader = new StreamReader(jsonPath);
            content = JsonConvert.DeserializeObject<List<Parking>>(reader.ReadToEnd());
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, $"Ошибка при загрузке {jsonPath}: {ex.Message}");
            throw;
        }
    }


    public List<Parking> GetAll()
    {
        return content.ToList();
    }

    public Parking GetById(int id)
    {
        return content.FirstOrDefault(x => x.Id == id);
    }
}
