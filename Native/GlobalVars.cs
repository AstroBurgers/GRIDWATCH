namespace GRIDWATCH.Native;
internal static class GlobalVars
{
    internal static Ped MainPlayer => Game.LocalPlayer.Character;
    internal static readonly Random Rndm = new(DateTime.Now.Millisecond);

    internal static bool IsWeatherInclement()
    {
        int currentWeather = NativeFunction.Natives.GET_PREV_WEATHER_TYPE_HASH_NAME<int>();
        return currentWeather == (int)WeatherType.Rain || currentWeather == (int)WeatherType.Thunder ||
               currentWeather == (int)WeatherType.Snow ||
               currentWeather == (int)WeatherType.Snowlight ||
               currentWeather == (int)WeatherType.Blizzard || currentWeather == (int)WeatherType.Xmas;
    }

    internal static float GetLightLevelModifier()
    {
        var hour = World.TimeOfDay.Hours;
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
        var dirt = vehicle.DirtLevel;

        return dirt switch
        {
            >= 10f => 0.65f, // basically "dirtiest truck ever"
            >= 5f => 0.8f, // dirty car
            >= 2f => 0.9f, // slightly dusty
            _ => 1f // clean
        };
    }
}