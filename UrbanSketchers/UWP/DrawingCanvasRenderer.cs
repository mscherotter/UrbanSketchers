using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using UrbanSketchers.Controls;
using Xamarin.Forms.Platform.UWP;
using UWP;

[assembly: ExportRenderer(typeof(DrawingCanvas), typeof(DrawingCanvasRenderer))]

namespace UWP
{
    
    public class DrawingCanvasRenderer : ViewRenderer<DrawingCanvas, DrawingControl>
    {
        DrawingControl _drawingControl = new DrawingControl();

        protected override void OnElementChanged(ElementChangedEventArgs<DrawingCanvas> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                e.NewElement.GetImageFunc = _drawingControl.GetImageAsync;

                SetNativeControl(_drawingControl);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "BackgroundColor")
            {
                var colorConverter = new ColorConverter();

                var backgroundColor = colorConverter.Convert(Element.BackgroundColor, null, null, null);

                _drawingControl.Background = new SolidColorBrush((Color) backgroundColor);    
            }
        }
    }
}
