using System.IO;

namespace UrbanSketchers.Interfaces
{
    /// <summary>
    ///     Drawing page interface
    /// </summary>
    public interface IDrawingPage
    {
        /// <summary>
        ///     Gets or sets the image stream
        /// </summary>
        Stream ImageStream { get; set; }

        /// <summary>
        ///     Gets or sets the ink stream
        /// </summary>
        MemoryStream InkStream { get; set; }
    }
}