using Nager.VideoStream;

namespace ParkingPlaces.CamerasWork;

public class RtspProviderConfig
{
    public string FfmpegPath { get; set; }
    public OutputImageFormat CameraImageFormat { get; set; }
    public string OutputImagePath { get; set; }
    public int SaveIntervalSeconds { get; set; }
}
