#region usings

using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;

#endregion

namespace Heath.Lister.Infrastructure.Interactivity
{
    public class UpdateSourceTextChangedBehavior : Behavior<TextBox>
    {
        private BindingExpression _expression;

        protected override void OnAttached()
        {
            base.OnAttached();

            _expression = AssociatedObject.GetBindingExpression(TextBox.TextProperty);
            AssociatedObject.TextChanged += TextChanged;
        }

        // TODO: I don't do this for any of the rest of the Behaviors, needed?
        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.TextChanged -= TextChanged;
            _expression = null;
        }

        private void TextChanged(object sender, EventArgs args)
        {
            _expression.UpdateSource();
        }
    }
}