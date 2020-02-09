using System.Collections.Generic;
using System.Linq;

namespace Concludo.Core
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Singleton<T>(T value)
            => Enumerable.Repeat(value, 1);
    }
}