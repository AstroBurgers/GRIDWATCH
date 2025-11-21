namespace GRIDWATCH.Features.Shotspotter;

internal static class SpawnProcess
{
    private static readonly WeaponHash[] GunTypes =
    [
        WeaponHash.Pistol,
        WeaponHash.CombatPistol,
        WeaponHash.Smg,
        WeaponHash.APPistol
    ];

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
                if (Rndm.Next(0, 100) <= UserConfig.ShotspotterChance)
                {
                    if (Rndm.Next(0, 100) <= UserConfig.ShotspotterFalseAlarmChance) {
                        SpawnFalseGunfireIncident();
                        return;
                    }
                    SpawnGunfireIncident();
                }

                GameFiber.Wait(UserConfig.ShotspotterPollRate);
            }
        });
    }

    private static void SpawnGunfireIncident()
    {
        try
        {
            // choose a random location near the player
            var pos = World.GetNextPositionOnStreet(MainPlayer.Position.Around2D(500f));

            // spawn a shooter with random facing
            var shooter = new Ped(pos, Rndm.Next(0, 360));
            shooter.Inventory.GiveNewWeapon(GunTypes[Rndm.Next(GunTypes.Length)], -1, true);

            switch (Rndm.Next(0, 101))
            {
                case <= 50:
                    shooter.Tasks.FightAgainst(shooter.GetNearbyPeds(16).OrderBy(p => p.Position.DistanceTo(shooter.Position)).FirstOrDefault(), 30000).WaitForCompletion();
                    shooter.Tasks.Wander();
                    break;
                case >= 49:
                    var target = new Vector3(
                        pos.X + Rndm.Next(-10, 10),
                        pos.Y + Rndm.Next(-10, 10),
                        pos.Z
                    );

                    shooter.Tasks.AimWeaponAt(target, 3000);
                    shooter.Tasks.FireWeaponAt(target, 5000, FiringPattern.FullAutomatic).WaitForCompletion();
                    shooter.Tasks.Wander();
                    break;
            }

            var incident = new GunfireIncident(
                loc: pos,
                shooter: shooter,
                weapon: shooter.Inventory.EquippedWeapon.ToString()
            );

            EventHub.Publish(incident);
        }
        catch (Exception ex)
        {
            Error(ex);
        }
    }

    private static void SpawnFalseGunfireIncident()
    {
        try
        {
            // choose a random location near the player
            var pos = World.GetNextPositionOnStreet(MainPlayer.Position.Around2D(500f));

            var incident = new GunfireIncident(
                loc: pos,
                shooter: null,
                weapon: string.Empty
            );

            EventHub.Publish(incident);
        }
        catch (Exception ex)
        {
            Error(ex);
        }
    }
}