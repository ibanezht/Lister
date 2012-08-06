#region usings

using System.IO.IsolatedStorage;

#endregion

namespace Heath.Lister.Infrastructure
{
    public class Setting<T>
    {
        private readonly T _defaultValue;
        private readonly string _name;
        private bool _hasValue;
        private T _value;

        public Setting(string name, T defaultValue)
        {
            _name = name;
            _defaultValue = defaultValue;
        }

        public T Value
        {
            get
            {
                if (!_hasValue)
                {
                    if (!IsolatedStorageSettings.ApplicationSettings.TryGetValue(_name, out _value))
                    {
                        _value = _defaultValue;
                        IsolatedStorageSettings.ApplicationSettings[_name] = _value;
                    }
                    _hasValue = true;
                }

                return _value;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings[_name] = value;
                _value = value;
                _hasValue = true;
            }
        }
    }
}