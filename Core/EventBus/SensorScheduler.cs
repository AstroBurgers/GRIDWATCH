using GRIDWATCH.Features.SharedSystems;

namespace GRIDWATCH.Core.EventBus;

internal static class SensorScheduler
{
    private static readonly List<ISensor> Sensors = [];

    public static void Register(ISensor sensor) => Sensors.Add(sensor);

    public static void Run()
    {
        GameFiber.StartNew(() =>
        {
            while (true)
            {
                var cameras = CameraFetcher.FetchNearbyCameras();
                foreach (var s in Sensors)
                    s.Tick(cameras);

                GameFiber.Wait(UserConfig.ScanInterval);
            }
        });
    }
}

internal interface ISensor
{
    void Tick(IEnumerable<Entity> cameras);
}