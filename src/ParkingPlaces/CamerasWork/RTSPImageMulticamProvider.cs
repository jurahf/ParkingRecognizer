using Microsoft.Extensions.Options;
using ParkingPlaces.Models;
using ParkingPlaces.Repositories;

namespace ParkingPlaces.CamerasWork;

/// <summary>
/// Получение изображений с набора камер, работающих по протоколу RTSP
/// </summary>
public class RTSPImageMulticamProvider : IImageProvider
{
    private readonly IRepository<Parking> parkingRepository;
    IOptions<RtspProviderConfig> config;
    private Dictionary<int, RTSPImageProvider> cameraProviders = new Dictionary<int, RTSPImageProvider>();

    public RTSPImageMulticamProvider(IRepository<Parking> repository, IOptions<RtspProviderConfig> config)
    {
        this.parkingRepository = repository;
        this.config = config;

        Start();
    }

    ~RTSPImageMulticamProvider()
    {
        Stop();
    }

    public string GetImagePath(Camera camera, out bool isOnline)
    {
        return cameraProviders[camera.Id].GetImagePath(camera, out isOnline);
    }


    private void Start()
    {
        //string json = JsonConvert.SerializeObject(repository.GetById(1));

        foreach (var camera in parkingRepository.GetById(1).Cameras)    // TODO: parking Id
        {
            cameraProviders.Add(camera.Id, new RTSPImageProvider(camera, config));
        }
    }

    private void Stop()
    {
    }
}
