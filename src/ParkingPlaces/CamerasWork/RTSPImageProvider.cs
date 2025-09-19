using Microsoft.Extensions.Options;
using Nager.VideoStream;
using ParkingPlaces.Models;
using Serilog;
using System.Text.RegularExpressions;

namespace ParkingPlaces.CamerasWork;

/// <summary>
/// Обеспечивает получение изображения с одной камеры, работающей по протоколу RTSP
/// </summary>
public class RTSPImageProvider : IImageProvider
{
    private readonly RtspProviderConfig config;
    private readonly Camera camera;
    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private VideoStreamClient client;
    private string outputImagePath;
    private DateTime lastSave = DateTime.MinValue;
    private TimeSpan saveInterval = TimeSpan.FromSeconds(1);    // временной интервал, через который сохраняется кадр с камеры
    private TimeSpan storageFilesForTime = TimeSpan.FromSeconds(60); // сколько хранить файлы (старше удаляются)

    public RTSPImageProvider(Camera camera, IOptions<RtspProviderConfig> config)
    {
        this.config = config.Value;
        this.camera = camera;
        outputImagePath = Path.Combine(config.Value.OutputImagePath, camera.Name);  // TODO: normalize name

        Log.Logger.Information($"Подключение камеры {camera.Name} по ссылке {camera.RtspUrl}");
        Start();
    }

    ~RTSPImageProvider()
    {
        Stop();
    }

    private void Start()
    {
        try
        {
            saveInterval = config.SaveIntervalSeconds > 0
                ? TimeSpan.FromSeconds(config.SaveIntervalSeconds)
                : TimeSpan.FromSeconds(1);
            storageFilesForTime = config.StorageFileForSeconds > 0
                ? TimeSpan.FromSeconds(config.StorageFileForSeconds)
                : TimeSpan.FromSeconds(60);

            if (!Directory.Exists(outputImagePath))
                Directory.CreateDirectory(outputImagePath);

            var inputSource = new StreamInputSource(camera.RtspUrl);

            client = new VideoStreamClient(string.Format(config.FfmpegPath, camera.Name)); // TODO: то есть должен существовать этот путь
            client.NewImageReceived += NewImageReceived;
            client.StartFrameReaderAsync(inputSource, config.CameraImageFormat, cancellationTokenSource.Token);

            Task.Run(() => DeleteOldFiles(cancellationTokenSource.Token));
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, $"Ошибка подключения камеры {camera.Name}: {ex.Message}");
        }
    }

    private async Task DeleteOldFiles(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.Now;
                List<string> forDel = Directory.GetFiles(outputImagePath)
                   .Where(x =>
                   {
                       string fileName = Path.GetFileNameWithoutExtension(x);
                       var timeStampMatch = Regex.Match(fileName, @"\d*");
                       if (timeStampMatch.Success)
                       {
                           return now - new DateTime(long.Parse(timeStampMatch.Value)) >= storageFilesForTime;
                       }
                       else
                       {
                           return true; // все посторонние файлы удаляем
                       }
                   })
                   .OrderBy(x => x)
                   .ToList();

                foreach (string file in forDel)
                {
                    File.Delete(file);
                }

                await Task.Delay(storageFilesForTime / 10, cancellationToken);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, $"Ошибка при удалении старых кадров с камеры {camera.Name}: {ex.Message}");
            }
        }
    }

    private void Stop()
    {
        if (client != null)
        {
            client.NewImageReceived -= NewImageReceived;
        }

        cancellationTokenSource.Cancel();
    }

    public string GetImagePath(Camera camera, out bool isOnline)
    {
        int tries = 0;
        string? result;

        do
        {
            result = Directory.GetFiles(outputImagePath)
                .Where(x =>
                {
                    string fileName = Path.GetFileNameWithoutExtension(x);
                    var timeStampMatch = Regex.Match(fileName, @"^\d*$");
                    return timeStampMatch.Success;
                })
                .OrderByDescending(x => x)
                .FirstOrDefault();

            if (result == null)
            {
                Task.Delay(200);
            }
        } while (result == null && tries++ < 5);

        isOnline = result != null;

        if (!isOnline)
        {
            Log.Logger.Warning($"Отсуствуют изображения для камеры {camera.Name}. Камера в режиме offline");
            result = "";
        }

        return result;
    }

    private void NewImageReceived(byte[] imageData)
    {
        var now = DateTime.Now;
        if (now - lastSave > saveInterval)
        {
            lastSave = now;
            File.WriteAllBytes($@"{outputImagePath}/{now.Ticks}.jpg", imageData);
        }
    }

}
