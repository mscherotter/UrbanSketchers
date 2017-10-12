using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace UrbanSketchers.Controls
{
    /// <summary>
    /// Drawing canvas
    /// </summary>
    public class DrawingCanvas : View
    {
        /// <summary>
        /// Initializes a new instance of the DrawingCanvas class.
        /// </summary>
        public DrawingCanvas()
        {
            BackgroundColor = Color.White;
        }

        /// <summary>
        /// the bitmap file format
        /// </summary>
        public enum BitmapFileFormat
        {
            /// <summary>
            /// Windows Bitmap format
            /// </summary>
            Bmp,
            /// <summary>
            /// Portable Network Graphics format
            /// </summary>
            Png,
            /// <summary>
            /// JPEG format
            /// </summary>
            Jpeg,
            /// <summary>
            /// TIFF format
            /// </summary>
            Tiff,
            /// <summary>
            /// GIF format
            /// </summary>
            Gif,
            /// <summary>
            /// JPEG XR Format
            /// </summary>
            JpegXR
        };

        /// <summary>
        /// The function to get an image of the ink in a specific format
        /// </summary>
        public Func<Stream, BitmapFileFormat, Task<bool>> GetImageFunc { get; set; }
    }
}
