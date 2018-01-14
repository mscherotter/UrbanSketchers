using Windows.UI.Xaml;
using UrbanSketchers.Controls;

namespace UWP
{
    /// <summary>
    ///     Map Icon properties
    /// </summary>
    public sealed class MapIconProperties
    {
        /// <summary>
        ///     the sketch pin dependency property
        /// </summary>
        public static readonly DependencyProperty SketchPinProperty =
            DependencyProperty.RegisterAttached("SketchPin", typeof(ISketchPin), typeof(MapIconProperties),
                new PropertyMetadata(null));

        /// <summary>
        ///     Gets the sketch pin
        /// </summary>
        /// <param name="obj">the dependency object</param>
        /// <returns>a sketch pin</returns>
        public static ISketchPin GetSketchPin(DependencyObject obj)
        {
            return (ISketchPin) obj.GetValue(SketchPinProperty);
        }

        /// <summary>
        ///     Sets the sketch pin
        /// </summary>
        /// <param name="obj">the dependency object</param>
        /// <param name="value">the sketch pin</param>
        public static void SetSketchPin(DependencyObject obj, ISketchPin value)
        {
            obj.SetValue(SketchPinProperty, value);
        }
    }
}