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
            while (true)
            {
                if (!Main.OnDuty)
                {
                    GameFiber.Wait(5000);
                    continue;
                }

                // 1 in N chance per minute to spawn a shot
                if (Rndm.Next(0, 100) < 5)
                {
                    SpawnGunfireIncident();
                }

                GameFiber.Wait(60000); // 1 minute scan
            }
        });
    }

    private static void SpawnGunfireIncident()
    {
        var pos = World.GetNextPositionOnStreet(MainPlayer.Position.Around2D(200f));
        var shooter = new Ped("s_m_y_cop_01", pos, Rndm.Next(0, 360));
        shooter.Inventory.GiveNewWeapon(GunTypes[Rndm.Next(GunTypes.Length)], -1, true);
        shooter.Tasks.AimWeaponAt(
            new Vector3(pos.X + Rndm.Next(-10, 10), pos.Y + Rndm.Next(-10, 10), pos.Z),
            3000
        );
        shooter.Tasks.FireWeaponAt(
            new Vector3(pos.X + Rndm.Next(-10, 10), pos.Y + Rndm.Next(-10, 10), pos.Z),
            5000,
            FiringPattern.FullAutomatic
        );

        var incident = new GunfireIncident(
            loc: pos,
            shooter: shooter,
            weapon: shooter.Inventory.EquippedWeapon.ToString()
        );
        
        // Log it for Gridwatch
        //GunfireEventRegistry.Report(new GunfireIncident(pos, shooter, shooter.Inventory.EquippedWeapon.Name));
    }
}