#region usings

using System.Windows;

#endregion

namespace Heath.Lister.Infrastructure
{
    public class ValueProvider : DependencyObject
    {
        private const string ValuePropertyName = "Value";

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                ValuePropertyName,
                typeof(string),
                typeof(ValueProvider), null);

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public override string ToString()
        {
            return Value;
        }
    }
}