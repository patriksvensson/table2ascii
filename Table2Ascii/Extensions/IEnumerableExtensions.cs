using Spectre.Console;
using System.Collections.Generic;
using System.Linq;

namespace TableToAscii
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> items, int count)
        {
            return items.Select((item, index) => new { item, index = index })
                .GroupBy(x => x.index / count)
                .Select(g => g.Select(x => x.item));
        }
    }
}
