using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml.Media;
using UrbanSketchers.Controls;
using UWP.Renderers;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(DrawingCanvas), typeof(DrawingCanvasRenderer))]

namespace UWP.Renderers
{
    /// <summary>
    ///     <see cref="DrawingCanvas" /> renderer for UWP platform
    /// </summary>
    public class DrawingCanvasRenderer : ViewRenderer<DrawingCanvas, DrawingControl>
    {
        /// <summary>
        ///     <see cref="DrawingCanvas" /> element changed.
        /// </summary>
        /// <param name="e">the element changed event arguments</param>
        protected override void OnElementChanged(ElementChangedEventArgs<DrawingCanvas> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                var control = new DrawingControl();

                SetNativeControl(control);

                e.NewElement.GetImageFunc = control.GetImageAsync;
            }
        }

        /// <summary>
        ///     DrawingCanvas property changed
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the property changed event arguments</param>
        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "BackgroundColor")
            {
                var colorConverter = new ColorConverter();

                var backgroundColor = colorConverter.Convert(Element.BackgroundColor, null, null, string.Empty);

                Control.Background = new SolidColorBrush((Color) backgroundColor);
            }
            if (e.PropertyName == "InkStream")
                await Control.LoadAsync(Element.InkStream);
        }
    }
}