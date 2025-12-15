using GRIDWATCH.Features.Cameras;
using GRIDWATCH.Features.Shotspotter;

namespace GRIDWATCH.API;

/// <summary>
///     Provides utility functions for interacting with license plate hits and gunfire incidents.
/// </summary>
public static class Functions
{
    /// <summary>
    ///     Retrieves all license plate hits from the event consumers.
    /// </summary>
    /// <returns>A list of <see cref="LicensePlateHit" /> objects representing all license plate hits.</returns>
    public static List<LicensePlateHit> GetAllLicensePlateHits()
    {
        return EventConsumers.GetAllAlprHits();
    }

    /// <summary>
    ///     Retrieves all gunfire incidents from the event consumers.
    /// </summary>
    /// <returns>A list of <see cref="GunfireIncident" /> objects representing all gunfire incidents.</returns>
    public static List<GunfireIncident> GetAllShotspotterAlerts()
    {
        return EventConsumers.GetAllGunfireIncidents();
    }

    /// <summary>
    ///     Checks if a specific vehicle, identified by its license plate, has been flagged.
    /// </summary>
    /// <param name="licensePlate">The license plate of the vehicle to check.</param>
    /// <returns><c>true</c> if the vehicle has been flagged; otherwise, <c>false</c>.</returns>
    public static bool HasVehicleBeenFlagged(string licensePlate)
    {
        return EventConsumers
            .GetAllAlprHits()
            .Any(i => i.LicensePlate.Equals(licensePlate, StringComparison.OrdinalIgnoreCase));
    }
}