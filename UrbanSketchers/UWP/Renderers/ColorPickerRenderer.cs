using Windows.Foundation.Metadata;
using UrbanSketchers.Controls;
using UWP.Renderers;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(ColorPickerView), typeof(ColorPickerRenderer))]

namespace UWP.Renderers
{
    /// <summary>
    ///     UWP ColorPicker Renderer
    /// </summary>
    public class ColorPickerRenderer : ViewRenderer<ColorPickerView, Windows.UI.Xaml.Controls.Control>
    {
        /// <summary>
        ///     Set the native control
        /// </summary>
        /// <param name="e">the element changed event arguments</param>
        protected override void OnElementChanged(ElementChangedEventArgs<ColorPickerView> e)
        {
            base.OnElementChanged(e);

            if (ApiInformation.IsTypePresent("Windows.UI.Xaml.Controls.ColorPicker"))
            {
                SetNativeControl(new Windows.UI.Xaml.Controls.ColorPicker());
            }
            else
            {
                SetNativeControl(new Coding4Fun.Toolkit.Controls.ColorPicker());
            }
        }
    }
}