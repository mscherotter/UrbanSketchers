using UrbanSketchers.Data;
using Xamarin.Forms.Maps;

namespace UrbanSketchers.Interfaces
{
    /// <summary>
    ///     Edit Sketch page interface
    /// </summary>
    public interface IEditSketchPage
    {
        /// <summary>
        ///     Gets or sets the sketch Id
        /// </summary>
        string SketchId { get; set; }

        /// <summary>
        /// Gets or sets the sketch
        /// </summary>
        ISketch Sketch { get; set; }
        Distance Radius { get; set; }
        MapType MapType { get; set; }
    }
}