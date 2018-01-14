using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrbanSketchers.Controls;
using Windows.UI.Xaml;

namespace UWP
{
    /// <summary>
    /// Map Icon properties
    /// </summary>
    public sealed class MapIconProperties
    {
        public static SketchPin GetSketchPin(DependencyObject obj)
        {
            return (SketchPin)obj.GetValue(SketchPinProperty);
        }

        public static void SetSketchPin(DependencyObject obj, SketchPin value)
        {
            obj.SetValue(SketchPinProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SketchPinProperty =
            DependencyProperty.RegisterAttached("SketchPin", typeof(SketchPin), typeof(MapIconProperties), new PropertyMetadata(null));





    }
}
