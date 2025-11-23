using GRIDWATCH.Features.SharedSystems;

namespace GRIDWATCH.Core.EventBus;

internal static class SensorScheduler
{
    private static readonly List<ISensor> Sensors = [];

    internal static void Register(ISensor sensor) => Sensors.Add(sensor);

    internal static void Run()
    {
        GameFiber.StartNew(Start, "GRIDWATCH Sensor Scheduler");
    }

    private static void Start()
    {
        while (true)
        {
            if (LSPD_First_Response.Mod.API.Functions.IsPlayerPerformingPullover()) {
                continue;
            }
            var cameras = CameraFetcher.FetchNearbyCameras();
            foreach (var s in Sensors) s.Tick(cameras);

            GameFiber.Wait(UserConfig.ScanInterval);
        }
    }
}

internal interface ISensor
{
    void Tick(IEnumerable<Entity> cameras);
}