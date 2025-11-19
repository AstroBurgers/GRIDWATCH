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

    internal static List<Entity> FetchNearbyCameras()
    {
        try
        {
            var playerPos = MainPlayer.Position;
            var worldCameras = World.GetAllEntities()
                .Where(p => CameraProps.Contains(p.Model.Hash)
                            && p.Position.DistanceTo(playerPos) < 200f)
                .ToList();

            Debug($"Fetched {worldCameras.Count} cameras");
            return worldCameras;
        }
        catch (Exception ex)
        {
            Error(ex);
        }

        return null;
    }
}