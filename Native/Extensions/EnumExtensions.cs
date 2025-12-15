namespace GRIDWATCH.Native.Extensions;

internal static class EnumExtensions
{
    // Thanks again, Khori
    internal static T PickRandom<T>(this IEnumerable<T> source)
    {
        List<T> list = source.ToList();
        return list.Any() ? list.PickRandom(1).Single() : default;
    }

    internal static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
    {
        return source.Shuffle().Take(count);
    }

    internal static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.OrderBy(_ => Guid.NewGuid());
    }
}