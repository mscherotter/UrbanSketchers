using System;
using Xamarin.Forms;

namespace UrbanSketchers.Controls
{
    /// <summary>
    ///     Image that supports connected animations
    /// </summary>
    public class ConnectedImage : Image
    {
        /// <summary>
        ///     Gets or sets the native control
        /// </summary>
        public object Control { get; set; }

        /// <summary>
        ///     Event to animate the destination control
        /// </summary>
        public event EventHandler<TypedEventArgs<string>> Animate;


        /// <summary>
        ///     Event to prepare the source control to animate
        /// </summary>
        public event EventHandler<TypedEventArgs<string>> PrepareToAnimate;

        /// <summary>
        ///     Start the connected animation
        /// </summary>
        /// <param name="name">the name of the animation</param>
        public void StartConnectedAnimation(string name)
        {
            Animate?.Invoke(Control, new TypedEventArgs<string>(name));
        }

        /// <summary>
        ///     Prepare to animate
        /// </summary>
        /// <param name="name">the animation name</param>
        public void Prepare(string name)
        {
            PrepareToAnimate?.Invoke(Control, new TypedEventArgs<string>(name));
        }
    }
}