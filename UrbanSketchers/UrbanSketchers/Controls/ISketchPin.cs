using System;
using System.Diagnostics.CodeAnalysis;
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
        [SuppressMessage("Design", "CA1056", Justification = "Necessary for serialization")]
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