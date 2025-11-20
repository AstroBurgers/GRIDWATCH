namespace GRIDWATCH.Utils;

internal static class GlobalVars
{
    internal static Ped MainPlayer => Game.LocalPlayer.Character;
    internal static readonly Random Rndm = new(DateTime.Now.Millisecond);
}