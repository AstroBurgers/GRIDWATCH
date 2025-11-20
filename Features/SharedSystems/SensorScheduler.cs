namespace GRIDWATCH.Features.SharedSystems;

internal static class SensorScheduler
{
    private static readonly List<ISensor> _sensors = new();

    public static void Register(ISensor sensor) => _sensors.Add(sensor);

    public static void Run()
    {
        GameFiber.StartNew(() =>
        {
            while (true)
            {
                var cameras = CameraFetcher.FetchNearbyCameras();
                foreach (var s in _sensors)
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