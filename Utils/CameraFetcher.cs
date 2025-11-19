using System.Linq;
using GRIDWATCH.Configs;
using static GRIDWATCH.Configs.CamerasIoHandler;
using static GRIDWATCH.Utils.GlobalVars;

namespace GRIDWATCH.Utils;

public class CameraFetcher
{
    internal static List<string> CameraProps = new()
    {
        "prop_traffic_01a",
        "prop_traffic_01d",
        "prop_traffic_03a",
        "prop_traffic_02b",
        "prop_traffic_01b"
    };
    
    internal static void FetchAndPrintAllCameras()
    {
        var WorldCameras = World.GetAllEntities().Where(i => i.Model.IsValid && CameraProps.Contains(i.Model.Name)).ToList();

        for (int i = 0; i < WorldCameras.Count(); i++)
        {
            var cameraData = new CameraData(id:i, new Position(WorldCameras[i].Position.X, WorldCameras[i].Position.Y,
                WorldCameras[i].Position.Z));
            Cameras.Add(cameraData);
        }
        
        WriteJson(Cameras);
    }
}