using GRIDWATCH.Native.Extensions;

namespace GRIDWATCH.Features.SharedSystems;

internal static class CameraFetcher
{
    private static readonly uint[] CameraProps =
    [
        0x3e2b73a4, 0x336e5e2a, 0xd8eba922, 0xd4729f50,
        0x272244b2, 0x33986eae, 0x2323cdc5
    ];

    private static readonly List<Entity> WorldCameras = [];

    internal static List<Entity> FetchNearbyCameras()
    {
        try
        {
            var all = World.GetAllEntities();
            WorldCameras.Clear();

            foreach (var e in all)
            {
                if (Array.IndexOf(CameraProps, e.Model.Hash) != -1)
                {
                    WorldCameras.Add(e);
                }
            }
            
            Debug($"Fetched {WorldCameras.Count} cameras");

            // Randomize and pick up to the user configured max number of cameras
            var randomCameras = WorldCameras.PickRandom(UserConfig.MaxCamerasPerScan).ToList();

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
            var all = World.GetAllEntities();
            Entity nearest = null;
            float nearestDistSq = float.MaxValue;

            foreach (var e in all)
            {
                uint h = (uint)e.Model.Hash;

                // camera model check
                for (int i = 0; i < CameraProps.Length; i++)
                {
                    if (CameraProps[i] != h) continue;
                    float distSq = e.Position.DistanceToSquared(position);

                    if (distSq < nearestDistSq)
                    {
                        nearestDistSq = distSq;
                        nearest = e;
                    }

                    break;
                }
            }

            Debug($"Fetched nearest camera: {nearest}");

            return nearest;
        }
        catch (Exception ex)
        {
            Error(ex);
            return null;
        }
    }
}