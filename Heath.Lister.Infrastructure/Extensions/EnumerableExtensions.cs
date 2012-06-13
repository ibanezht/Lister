#region usings

using System;
using System.Collections.Generic;

#endregion

namespace Heath.Lister.Infrastructure.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> e, Action<T> action)
        {
            foreach (var item in e)
            {
                action(item);
            }
        }
    }
}