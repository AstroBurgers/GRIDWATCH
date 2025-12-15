using GRIDWATCH.Features.SharedSystems;
using LSPD_First_Response.Mod.API;

namespace GRIDWATCH.Core.EventBus;

internal static class SensorScheduler
{
    private static readonly List<ISensor> Sensors = [];

    internal static void Register(ISensor sensor)
    {
        Sensors.Add(sensor);
    }

    internal static void Run()
    {
        GameFiber.StartNew(Start, "GRIDWATCH Sensor Scheduler");
    }

    private static void Start()
    {
        while (true)
        {
            GameFiber.Wait(UserConfig.ScanInterval);
            if (Functions.IsPlayerPerformingPullover()) continue;
            List<Entity> cameras = CameraFetcher.FetchNearbyCameras();
            foreach (ISensor s in Sensors) s.Tick(cameras);
        }
        // ReSharper disable once FunctionNeverReturns
    }
}

internal interface ISensor
{
    void Tick(IEnumerable<Entity> cameras);
}