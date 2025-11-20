using GRIDWATCH.Configs;

public class CameraFetcher
{
    internal static readonly List<uint> CameraProps = new()
    {
        0x3e2b73a4, 0x336e5e2a, 0xd8eba922, 0xd4729f50,
        0x272244b2, 0x33986eae, 0x2323cdc5
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

            // Randomize and pick up to 5
            var randomCameras = worldCameras
                .OrderBy(_ => Rndm.Next())
                .Take(5)
                .ToList();

            return randomCameras;
        }
        catch (Exception ex)
        {
            Error(ex);
            return new List<Entity>();
        }
    }
}