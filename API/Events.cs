using GRIDWATCH.Features.Cameras;
using GRIDWATCH.Features.Shotspotter;

namespace GRIDWATCH.API;

/// <summary>
///     Provides static events for handling specific incidents such as license plate hits and gunfire incidents.
/// </summary>
public static class Events
{
    /// <summary>
    ///     Event triggered when a new license plate hit is added.
    /// </summary>
    public static event Action<LicensePlateHit> OnLicensePlateHitAdded;

    /// <summary>
    ///     Event triggered when a new gunfire incident is added.
    /// </summary>
    public static event Action<GunfireIncident> OnGunfireIncidentAdded;

    /// <summary>
    ///     Invokes the <see cref="OnLicensePlateHitAdded" /> event with the provided license plate hit data.
    /// </summary>
    /// <param name="hit">The license plate hit data to be passed to the event.</param>
    internal static void LicensePlateHitAdded(LicensePlateHit hit)
    {
        OnLicensePlateHitAdded?.Invoke(hit);
    }

    /// <summary>
    ///     Invokes the <see cref="OnGunfireIncidentAdded" /> event with the provided gunfire incident data.
    /// </summary>
    /// <param name="incident">The gunfire incident data to be passed to the event.</param>
    internal static void GunfireIncidentAdded(GunfireIncident incident)
    {
        OnGunfireIncidentAdded?.Invoke(incident);
    }
}