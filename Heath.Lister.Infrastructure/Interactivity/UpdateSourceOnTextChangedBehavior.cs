#region usings

using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;

#endregion

namespace Heath.Lister.Infrastructure.Interactivity
{
    public class UpdateSourceOnTextChangedBehavior : Behavior<TextBox>
    {
        private BindingExpression _expression;

        protected override void OnAttached()
        {
            base.OnAttached();

            _expression = AssociatedObject.GetBindingExpression(TextBox.TextProperty);
            AssociatedObject.TextChanged += OnTextChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.TextChanged -= OnTextChanged;
            _expression = null;
        }

        private void OnTextChanged(object sender, EventArgs args)
        {
            _expression.UpdateSource();
        }
    }
}