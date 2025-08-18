using ParkingPlaces.Models;
using ParkingPlaces.Repositories;
using SkiaSharp;

namespace ParkingPlaces.Stubs;

public class ParkingStubRepository : IRepository<Parking>
{
    public List<Parking> GetAll()
    {
        return new List<Parking>() { GetById(1) };
    }

    public Parking GetById(int id)
    {
        var parking = new Parking() { Id = id };

        var camera1 = new Camera()
        {
            Id = 1,
            Name = "cam1",
            RtspUrl = "rtsp://uatr:er48tp@192.168.0.5:554/11",
            ParkingId = parking.Id,
        };

        //камера 1, место 1
        for (int i = 1; i <= presetRects.Count; i++)
        {
            var place = new Place()
            {
                Id = i,
                ParkingId = parking.Id,
            };

            var viewedPlace = new ViewedPlace()
            {
                Id = i,
                CameraId = camera1.Id,
                PlaceId = i,
                Place = place,
                X1 = presetRects[i - 1].Left,
                Y1 = presetRects[i - 1].Top,
                X2 = presetRects[i - 1].Right,
                Y2 = presetRects[i - 1].Bottom,
            };

            parking.Places.Add(place);
            camera1.ViewedPlaces.Add(viewedPlace);
        }

        parking.Cameras.Add(camera1);

        return parking;
    }

    private List<SKRect> presetRects = new List<SKRect>()
    {
        new SKRect(686 ,883 ,759 ,961), // 0
        new SKRect(731 ,912 ,804 ,990),
        new SKRect(776 ,941 ,849 ,1019),
        new SKRect(821 ,970 ,894 ,1048),
        new SKRect(856 ,1010 ,929 ,1087),
        new SKRect(911 ,1028    ,984 ,1106),
        new SKRect(956 ,1057    ,1029    ,1135),
        new SKRect(1001    ,1086    ,1074    ,1164),
        new SKRect(1051    ,1115    ,1124    ,1193),
        new SKRect(1101    ,1144    ,1174    ,1222),
        new SKRect(1151    ,1173    ,1234    ,1251),


        new SKRect(178 ,786 ,251 ,864), // 11
        new SKRect(211 ,806 ,284 ,884),
        new SKRect(244 ,826 ,317 ,904),
        new SKRect(277 ,846 ,350 ,924),
        new SKRect(310 ,866 ,383 ,944),
        new SKRect(348 ,886 ,421 ,964),
        //new SKRect(376 ,906 ,449 ,984),
        new SKRect(399 ,926 ,472 ,1004),
        //new SKRect(442 ,946 ,515 ,1024),
        //new SKRect(475 ,966 ,548 ,1044),
        new SKRect(498 ,986 ,571 ,1064),
        new SKRect(564 ,1036    ,637 ,1114),
        new SKRect(609 ,1066    ,682 ,1144),
        new SKRect(654 ,1096    ,727 ,1174),
        new SKRect(699 ,1126    ,772 ,1204),
        new SKRect(744 ,1156    ,817 ,1234),
        new SKRect(789 ,1186    ,862 ,1264),
    };
}


