namespace GRIDWATCH.Features.Shotspotter;

internal struct GunfireIncident(Vector3 loc, Ped shooter, string weapon, bool hit = false)
{
    internal Vector3 Location { get; } = loc;
    internal Ped Shooter { get; } = shooter;
    internal bool ConfirmedHit { get; } = hit;
    internal string Weapon { get; } = weapon;
    internal DateTime Timestamp { get; } = DateTime.Now;
}