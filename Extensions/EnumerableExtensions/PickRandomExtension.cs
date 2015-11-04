using System.Collections.Generic;
using System.Linq;

namespace ManoSoftware.Extensions.EnumerableExtensions
{
    public static class PickRandomExtension
    {
        public static T PickRandom<T>(this IEnumerable<T> source)
        {
            return source.PickRandom(1).Single();
        }

        public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
        {
            return source.Shuffle().Take(count);
        }
    }
}
