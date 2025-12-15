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
                    if (Rndm.Next(0, 100) <= UserConfig.ShotspotterFalseAlarmChance)
                    {
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
            Vector3 pos = World.GetNextPositionOnStreet(MainPlayer.Position.Around2D(500f));

            // spawn a shooter with random facing
            Ped shooter = new(pos, Rndm.Next(0, 360));
            shooter.Inventory.GiveNewWeapon(GunTypes[Rndm.Next(GunTypes.Length)], -1, true);

            switch (Rndm.Next(0, 101))
            {
                case <= 50:
                    Ped[] nearbyPeds = shooter.GetNearbyPeds(16);
                    Ped victim = nearbyPeds[0];
                    shooter.Tasks.FightAgainst(victim, 30000).WaitForCompletion();
                    shooter.Tasks.Wander();
                    break;
                case >= 49:
                    shooter.Tasks.Wander();
                    break;
            }

            GunfireIncident incident = new(
                pos,
                shooter,
                shooter.Inventory.EquippedWeapon.ToString()
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
            Vector3 pos = World.GetNextPositionOnStreet(MainPlayer.Position.Around2D(500f));

            GunfireIncident incident = new(
                pos,
                null,
                string.Empty
            );

            EventHub.Publish(incident);
        }
        catch (Exception ex)
        {
            Error(ex);
        }
    }
}