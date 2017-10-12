using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace UrbanSketchers.Controls
{
    public class DrawingCanvas : View
    {
        public enum BitmapFileFormat
        {
            Bmp,
            Png,
            Jpeg,
            Tiff,
            Gif,
            JpegXR
        };

        public Func<Stream, BitmapFileFormat, Task<bool>> GetImageFunc { get; set; }
    }
}
