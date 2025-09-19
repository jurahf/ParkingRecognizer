using IVilson.AI.Yolov7net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ParkingPlaces.CamerasWork;
using ParkingPlaces.Models;
using ParkingPlaces.Repositories;
using Recognizer.Drawing;
using Recognizer.Recognizing;
using Serilog;

namespace ParkingTelegramBot;


internal class Program
{
    static async Task Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        // сервисы

        // запуск сохранения/удаления картинок с камер
        builder.Services.AddScoped<IImageProvider, RTSPImageMulticamProvider>();
        builder.Services.Configure<RtspProviderConfig>(builder.Configuration.GetSection("RtspProvider"));

        // сервисы для распознавания
        builder.Services.AddScoped<IYoloFactory, YoloFactory>();
        builder.Services.AddScoped<IResultsDrawer, ResultsDrawer>();
        builder.Services.AddScoped<IParkingRecognizer, ParkingRecognizer>();
        builder.Services.AddMemoryCache();

        // репозитории
        builder.Services.AddScoped<IRepository<Parking>, ParkingJsonRepository>();

        // сервисы бота
        builder.Services.AddScoped<ITelegramBot, TelegramBot>();
        builder.Services.Configure<TelegramConfig>(builder.Configuration.GetSection("TelegramSettings"));

        // логгер
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();
        builder.Logging.AddSerilog();

        // запуск бота
        builder.Services.AddHostedService<TelegramBotHostedService>();
        using IHost host = builder.Build();
        await host.RunAsync();

        // TODO: 

        // проверить, по каким камерам реально можно распознать

        // начинает тормозить, когда фоток много

        // проверить все TODO

        // не начинать второй раз поиск, если для пользователя сообщение уже обрабатывается
    }
}
