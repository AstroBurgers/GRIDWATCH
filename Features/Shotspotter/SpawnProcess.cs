using GRIDWATCH.Core.EventBus;

namespace GRIDWATCH.Features.Shotspotter;

internal static class SpawnProcess
{
    private static readonly WeaponHash[] GunTypes =
    {
        WeaponHash.Pistol,
        WeaponHash.CombatPistol,
        WeaponHash.Smg,
        WeaponHash.APPistol
    };

    internal static void Start()
    {
        GameFiber.StartNew(() =>
        {
            Normal("ShotSpotter simulation loop started.");

            while (true)
            {
                if (!Main.OnDuty)
                {
                    GameFiber.Wait(5000);
                    continue;
                }

                // Simulate random gunfire activity
                if (Rndm.Next(0, 100) < 5)
                {
                    SpawnGunfireIncident();
                }

                GameFiber.Wait(60000); // 1-minute loop interval
            }
        });
    }

    private static void SpawnGunfireIncident()
    {
        try
        {
            // choose a random location near the player
            var pos = World.GetNextPositionOnStreet(MainPlayer.Position.Around2D(200f));

            // spawn a shooter with random facing
            var shooter = new Ped(pos, Rndm.Next(0, 360));
            shooter.Inventory.GiveNewWeapon(GunTypes[Rndm.Next(GunTypes.Length)], -1, true);

            // simple firing animation simulation
            var target = new Vector3(
                pos.X + Rndm.Next(-10, 10),
                pos.Y + Rndm.Next(-10, 10),
                pos.Z
            );

            shooter.Tasks.AimWeaponAt(target, 3000);
            shooter.Tasks.FireWeaponAt(target, 5000, FiringPattern.FullAutomatic);

            // Build a structured event
            var incident = new GunfireIncident(
                loc: pos,
                shooter: shooter,
                weapon: shooter.Inventory.EquippedWeapon.ToString()
            );

            // 🧠 Publish to your central event hub
            EventHub.Publish(incident);
        }
        catch (Exception ex)
        {
            Error(ex);
        }
    }
}