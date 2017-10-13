using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml.Media;
using UrbanSketchers.Controls;
using UWP;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(DrawingCanvas), typeof(DrawingCanvasRenderer))]

namespace UWP
{
    /// <summary>
    /// <see cref="DrawingCanvas"/> renderer for UWP platform
    /// </summary>
    public class DrawingCanvasRenderer : ViewRenderer<DrawingCanvas, DrawingControl>
    {
        private readonly DrawingControl _drawingControl = new DrawingControl();

        /// <summary>
        /// <see cref="DrawingCanvas"/> element changed.
        /// </summary>
        /// <param name="e">the element changed event arguments</param>
        protected override void OnElementChanged(ElementChangedEventArgs<DrawingCanvas> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                e.NewElement.GetImageFunc = _drawingControl.GetImageAsync;

                SetNativeControl(_drawingControl);
            }
        }

        /// <summary>
        /// DrawingCanvas property changed
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the property changed event arguments</param>
        protected async override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "BackgroundColor")
            {
                var colorConverter = new ColorConverter();

                var backgroundColor = colorConverter.Convert(Element.BackgroundColor, null, null, string.Empty);

                _drawingControl.Background = new SolidColorBrush((Color) backgroundColor);
            }
            if (e.PropertyName == "InkStream")
            {
                await _drawingControl.LoadAsync(Element.InkStream);
            }
        }
    }
}