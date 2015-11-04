using System;
using System.Collections.Generic;
using System.Linq;

namespace ManoSoftware.Extensions.EnumerableExtensions
{
    public static class ShuffleExtension
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(x => Guid.NewGuid());
        }
    }
}
