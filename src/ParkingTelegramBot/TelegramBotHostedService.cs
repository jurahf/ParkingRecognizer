using Microsoft.Extensions.Hosting;

namespace ParkingTelegramBot
{
    internal class TelegramBotHostedService : IHostedService
    {
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ITelegramBot bot;

        public TelegramBotHostedService(ITelegramBot bot)
        {
            this.bot = bot;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            cancellationTokenSource = new CancellationTokenSource();

            await bot.Start(cancellationTokenSource.Token);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            cancellationTokenSource.Cancel();

            return Task.CompletedTask;
        }
    }
}
