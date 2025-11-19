using System.Diagnostics;
using System.Linq;
using GRIDWATCH.Configs;
using Rage.Attributes;

public class CameraFetcher
{
    internal static readonly List<uint> CameraProps = new()
    {
        Game.GetHashKey("prop_traffic_01a"),
        Game.GetHashKey("prop_traffic_01d"),
        Game.GetHashKey("prop_traffic_03a"),
        Game.GetHashKey("prop_traffic_02b"),
        Game.GetHashKey("prop_traffic_01b")
    };

    [ConsoleCommand("**DEVELOPMENT ONLY! DO NOT USE**")]
    internal static void FetchAndPrintAllCameras()
    {
        Warn("⚠️ Running camera fetcher! Expect temporary lag.");
        var sw = Stopwatch.StartNew();

        try
        {
            var worldCameras = World.GetAllEntities()
                .Where(e => e.Model.IsValid && CameraProps.Contains(e.Model.Hash))
                .ToList();

            var newList = new List<CameraData>(worldCameras.Count);
            for (int i = 0; i < worldCameras.Count; i++)
            {
                var pos = worldCameras[i].Position;
                newList.Add(new CameraData(
                    id: i,
                    new Position(pos.X, pos.Y, pos.Z)
                ));
            }

            CamerasIoHandler.WriteJson(newList);
            Info($"Fetched and wrote {newList.Count} cameras in {sw.ElapsedMilliseconds}ms.");
        }
        catch (Exception ex)
        {
            Warn($"Camera fetch failed: {ex.Message}");
        }
        finally
        {
            sw.Stop();
        }
    }
}