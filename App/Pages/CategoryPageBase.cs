using System;
using Terminal.Gui;

namespace TuiSample.App.Pages
{
    /// Base class for right-hand pages. ESC returns to Categories.
    public abstract class CategoryPageBase : View
    {
        private readonly Action _returnToCategories;

        protected CategoryPageBase(Action returnToCategories)
        {
            _returnToCategories = returnToCategories;
            X = 0; Y = 0; Width = Dim.Fill(); Height = Dim.Fill();

            KeyPress += e =>
            {
                if (e.KeyEvent.Key == Key.Esc)
                {
                    _returnToCategories();
                    e.Handled = true;
                }
            };
        }

        /// Call when the page action completes. Shell shows outcome then snaps back.
        protected void Complete(string message, Action<string> onComplete)
        {
            onComplete(message);
        }
    }
}
