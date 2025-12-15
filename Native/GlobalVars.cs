using static GRIDWATCH.Native.Enums.WeatherType;

namespace GRIDWATCH.Native;

internal static class GlobalVars
{
    internal static readonly Random Rndm = new(DateTime.Now.Millisecond);
    internal static Ped MainPlayer => Game.LocalPlayer.Character;

    internal static bool IsWeatherInclement()
    {
        int currentWeather = NativeFunction.Natives.GET_PREV_WEATHER_TYPE_HASH_NAME<int>();
        return currentWeather is (int)Rain or (int)Thunder or (int)Snow or (int)Snowlight or (int)Blizzard or (int)Xmas;
    }

    internal static float GetLightLevelModifier()
    {
        int hour = World.TimeOfDay.Hours;
        return hour switch
        {
            >= 23 or < 5 => 0.7f, // Dark night
            >= 5 and < 7 or >= 20 and < 23 => 0.85f, // Dusk / Dawn
            _ => 1.0f
        };
    }

    internal static float GetDirtModifier(Vehicle vehicle)
    {
        // 0.0f (clean) to ~15.0f (filthy)
        float dirt = vehicle.DirtLevel;

        return dirt switch
        {
            >= 10f => 0.65f, // basically "dirtiest truck ever"
            >= 5f => 0.8f, // dirty car
            >= 2f => 0.9f, // slightly dusty
            _ => 1f // clean
        };
    }
}