using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using UrbanSketchers.Controls;
using UWP;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using UrbanSketchers;

[assembly: ExportRenderer(typeof(ConnectedImage), typeof(ConnectedImageRenderer))]

namespace UWP
{
    /// <summary>
    /// Connected Animation image renderer
    /// </summary>
    public class ConnectedImageRenderer : ImageRenderer
    {
        /// <summary>
        /// Attach the Animate event handler and the Control property
        /// </summary>
        /// <param name="e">the element changed event arguments</param>
        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement is ConnectedImage connectedImage)
            {
                connectedImage.Animate += Animate;
                connectedImage.PrepareToAnimate += Prepare;

                var nativeImage = this.GetNativeElement();

                connectedImage.Control = nativeImage;
            }
        }

        private void Prepare(object sender, TypedEventArgs<string> args)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.Xaml.Media.Animation.ConnectedAnimationService") && this.Control is UIElement uiElement)
            {
                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate(args.Value, uiElement);
            }
        }

        /// <summary>
        /// Animate the 
        /// </summary>
        /// <param name="sender">the destination as a <see cref="UIElement"/></param>
        /// <param name="args">the event arguments with the name of the animation</param>
        private async void Animate(object sender, TypedEventArgs<string> args)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.Xaml.Media.Animation.ConnectedAnimationService"))
            {
                await Control.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, delegate
                {
                    var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation(args.Value);

                    if (animation != null && sender is UIElement destination)
                    {
                        animation.TryStart(destination);
                    }
                });
            }
        }
    }
}
