using System;
using Xamarin.Forms.Maps;

namespace UrbanSketchers.Controls
{
    /// <summary>
    ///     Sketch pin interface
    /// </summary>
    public interface ISketchPin
    {
        /// <summary>
        ///     Gets or sets the Xamarin fForms pin
        /// </summary>
        Pin Pin { get; set; }

        /// <summary>
        ///     Gets or sets the URL string
        /// </summary>
        string Url { get; set; }

        /// <summary>
        ///     the clicked event handler
        /// </summary>
        event EventHandler Clicked;

        /// <summary>
        ///     Invoke the clicked event
        /// </summary>
        void InvokeClick();
    }
}