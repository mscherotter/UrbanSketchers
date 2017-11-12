using System;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using UrbanSketchers;
using UrbanSketchers.Controls;
using UWP.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(ConnectedImage), typeof(ConnectedImageRenderer))]

namespace UWP.Renderers
{
    /// <summary>
    ///     Connected Animation image renderer
    /// </summary>
    public class ConnectedImageRenderer : ImageRenderer
    {
        /// <summary>
        ///     Attach the Animate event handler and the Control property
        /// </summary>
        /// <param name="e">the element changed event arguments</param>
        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement is ConnectedImage connectedImage)
            {
                connectedImage.Animate += Animate;
                connectedImage.PrepareToAnimate += Prepare;

                var nativeImage = GetNativeElement();

                connectedImage.Control = nativeImage;

                nativeImage.SetValue(NameProperty, connectedImage.Name);
            }
        }

        private void Prepare(object sender, TypedEventArgs<string> args)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.Xaml.Media.Animation.ConnectedAnimationService") &&
                Control is UIElement uiElement)
                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate(args.Value, uiElement);
        }

        /// <summary>
        ///     Animate the
        /// </summary>
        /// <param name="sender">the destination as a <see cref="UIElement" /></param>
        /// <param name="args">the event arguments with the name of the animation</param>
        private async void Animate(object sender, TypedEventArgs<string> args)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.Xaml.Media.Animation.ConnectedAnimationService"))
                await Control.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, delegate
                {
                    var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation(args.Value);

                    if (animation != null && sender is UIElement destination)
                        animation.TryStart(destination);
                });
        }
    }
}