using System;
using Xamarin.Forms.Maps;

namespace UrbanSketchers.Controls
{
    /// <summary>
    ///     A sketch pin
    /// </summary>
    public class SketchPin : ISketchPin
    {
        /// <summary>
        ///     Gets or sets the pin
        /// </summary>
        public Pin Pin { get; set; }

        /// <summary>
        ///     Gets or sets the URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        ///     Clicked event handler
        /// </summary>
        public event EventHandler Clicked;

        /// <summary>
        ///     invoke a click
        /// </summary>
        public void InvokeClick()
        {
            Clicked?.Invoke(this, new EventArgs());
        }
    }
}