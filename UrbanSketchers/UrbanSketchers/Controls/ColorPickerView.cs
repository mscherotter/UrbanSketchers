using Xamarin.Forms;

namespace UrbanSketchers.Controls
{
    /// <summary>
    ///     Color Picker View
    /// </summary>
    public class ColorPickerView : View
    {
        /// <summary>
        ///     Color dependency property
        /// </summary>
        public static readonly BindableProperty ColorProperty =
            BindableProperty.Create("Color", typeof(Color), typeof(ColorPickerView), Color.Black);

        /// <summary>
        ///     Gets or sets the color
        /// </summary>
        public Color Color
        {
            get => (Color) GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }
    }
}