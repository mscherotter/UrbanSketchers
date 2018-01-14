using System.Collections.ObjectModel;
using Xamarin.Forms.Maps;

namespace UrbanSketchers.Controls
{
    /// <summary>
    ///     A map with custom rendering for sketch images
    /// </summary>
    public class SketchMap : Map
    {
        /// <summary>
        ///     Initializes a new instance of the SketchMap control.
        /// </summary>
        public SketchMap()
        {
            CustomPins = new ObservableCollection<ISketchPin>();
        }

        /// <summary>
        ///     Gets or sets the custom pins
        /// </summary>
        public ObservableCollection<ISketchPin> CustomPins { get; set; }

        /// <summary>
        ///     Gets or sets the maximum image size (default is 300.0)
        /// </summary>
        public double MaxImageSize { get; set; } = 300.0;
    }
}