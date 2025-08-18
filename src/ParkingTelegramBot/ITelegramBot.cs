namespace ParkingTelegramBot
{
    public interface ITelegramBot
    {
        Task Start(CancellationToken cancellationToken);
    }
}
