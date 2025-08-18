namespace ParkingPlaces.Repositories
{
    public interface IRepository<T>
    {
        List<T> GetAll();

        T GetById(int id);
    }
}
