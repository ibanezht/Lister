#region usings

using System.Collections.Generic;
using Microsoft.Phone.Shell;

#endregion

namespace Heath.Lister.Infrastructure
{
    public static class TombstoningHelper
    {
        private static readonly IDictionary<string, object> _state = PhoneApplicationService.Current.State;

        public static void Save(string key, object value)
        {
            if (_state.ContainsKey(key))
                _state.Remove(key);

            _state.Add(key, value);
        }

        public static T Load<T>(string key)
        {
            object result;

            if (_state.TryGetValue(key, out result))
                _state.Remove(key);
            else
                result = default(T);

            return (T)result;
        }

        public static void Clear()
        {
            _state.Clear();
        }
    }
}