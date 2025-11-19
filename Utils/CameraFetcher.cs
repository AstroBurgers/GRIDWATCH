using GRIDWATCH.Configs;

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

    internal static List<CameraData> FetchNearbyCameras()
    {
        try
        {
            var playerPos = MainPlayer.Position;
            var worldCameras = World.GetAllObjects()
                .Where(p => CameraProps.Contains(p.Model.Hash)
                            && p.Position.DistanceTo(playerPos) < 200f)
                .ToList();

            var newList = new List<CameraData>(worldCameras.Count);
            newList.AddRange(worldCameras.Select(t => t.Position).Select((pos, i) => new CameraData(id: i, new Position(pos.X, pos.Y, pos.Z))));

            Debug($"Fetched {newList.Count} cameras");
            return newList;
        }
        catch (Exception ex)
        {
            Error(ex);
        }

        return null;
    }
}