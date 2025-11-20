namespace GRIDWATCH.Features.SharedSystems;

internal static class CameraFetcher
{
    private static readonly uint[] CameraProps =
    [
        0x3e2b73a4, 0x336e5e2a, 0xd8eba922, 0xd4729f50,
        0x272244b2, 0x33986eae, 0x2323cdc5
    ];

    internal static List<Entity> FetchNearbyCameras()
    {
        try
        {
            var worldCameras = World.GetAllEntities()
                .Where(p => CameraProps.Contains(p.Model.Hash))
                .ToList();

            Debug($"Fetched {worldCameras.Count} cameras");

            // Randomize and pick up to the user configured max number of cameras
            var randomCameras = worldCameras
                .OrderBy(_ => Rndm.Next())
                .Take(UserConfig.MaxCamerasPerScan)
                .ToList();

            return randomCameras;
        }
        catch (Exception ex)
        {
            Error(ex);
            return [];
        }
    }
    
    internal static Entity FetchNearestCamera(Vector3 position)
    {
        try
        {
            var worldCameras = World.GetAllEntities()
                .Where(p => CameraProps.Contains(p.Model.Hash))
                .ToList();

            Debug($"Fetched {worldCameras.Count} cameras for nearest search");

            var nearestCamera = worldCameras
                .OrderBy(cam => cam.Position.DistanceTo(position))
                .FirstOrDefault();

            return nearestCamera;
        }
        catch (Exception ex)
        {
            Error(ex);
            return null;
        }
    }
}