#region usings

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Heath.Lister.Infrastructure
{
    public static class EnumValueCache
    {
        private static readonly IDictionary<Type, object[]> _cache = new Dictionary<Type, object[]>();

        public static object[] GetValues(Type type)
        {
            object[] retval;

            if (!_cache.TryGetValue(type, out retval))
            {
                retval = type.GetFields()
                    .Where(f => f.IsLiteral)
                    .Select(f => f.GetValue(null))
                    .ToArray();

                _cache[type] = retval;
            }

            return retval;
        }
    }
}