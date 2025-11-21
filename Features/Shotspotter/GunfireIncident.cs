namespace GRIDWATCH.Features.Shotspotter;

public readonly struct GunfireIncident(Vector3 loc, Ped shooter, string weapon, bool hit = false)
{
    public Vector3 Location { get; } = loc;
    public Ped Shooter { get; } = shooter;
    public bool ConfirmedHit { get; } = hit;
    public string Weapon { get; } = weapon;
    public DateTime Timestamp { get; } = DateTime.Now;
}