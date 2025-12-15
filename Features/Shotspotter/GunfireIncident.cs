namespace GRIDWATCH.Features.Shotspotter;

/// <summary>
///     Represents a gunfire incident detected by the system.
/// </summary>
/// <param name="loc">The location where the gunfire incident occurred.</param>
/// <param name="shooter">The shooter involved in the gunfire incident.</param>
/// <param name="weapon">The weapon used in the gunfire incident.</param>
/// <param name="hit">Indicates whether the gunfire resulted in a confirmed hit (default is false).</param>
public readonly struct GunfireIncident(Vector3 loc, Ped shooter, string weapon, bool hit = false)
{
    /// <summary>
    ///     Gets the location where the gunfire incident occurred.
    /// </summary>
    public Vector3 Location { get; } = loc;

    /// <summary>
    ///     Gets the shooter involved in the gunfire incident.
    /// </summary>
    public Ped Shooter { get; } = shooter;

    /// <summary>
    ///     Gets a value indicating whether the gunfire resulted in a confirmed hit.
    /// </summary>
    public bool ConfirmedHit { get; } = hit;

    /// <summary>
    ///     Gets the weapon used in the gunfire incident.
    /// </summary>
    public string Weapon { get; } = weapon;

    /// <summary>
    ///     Gets the timestamp when the gunfire incident was recorded.
    /// </summary>
    public DateTime Timestamp { get; } = DateTime.Now;
}