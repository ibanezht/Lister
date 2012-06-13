#region usings

using Microsoft.Phone.Shell;

#endregion

namespace Heath.Lister.Infrastructure
{
    public static class TombstoningHelper
    {
        public static void Save(string key, object value)
        {
            if (PhoneApplicationService.Current.State.ContainsKey(key))
                PhoneApplicationService.Current.State.Remove(key);

            PhoneApplicationService.Current.State.Add(key, value);
        }

        public static T Load<T>(string key)
        {
            object result;

            if (PhoneApplicationService.Current.State.TryGetValue(key, out result))
                PhoneApplicationService.Current.State.Remove(key);
            else
                result = default(T);

            return (T)result;
        }

        public static void Clear()
        {
            PhoneApplicationService.Current.State.Clear();
        }
    }
}