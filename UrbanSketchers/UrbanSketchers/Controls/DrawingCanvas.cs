using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace UrbanSketchers.Controls
{
    /// <summary>
    ///     Drawing canvas
    /// </summary>
    public class DrawingCanvas : View
    {
        private MemoryStream _inkStream;

        /// <summary>
        ///     Initializes a new instance of the DrawingCanvas class.
        /// </summary>
        public DrawingCanvas()
        {
            BackgroundColor = Color.White;
        }

        /// <summary>
        ///     The function to get an image of the ink in a specific format
        /// </summary>
        public Func<Stream, DrawingFileFormat, Task<bool>> GetImageFunc { get; set; }

        /// <summary>
        ///     Gets or sets the ISF ink stream
        /// </summary>
        public MemoryStream InkStream
        {
            get => _inkStream;
            set
            {
                if (_inkStream != value)
                {
                    _inkStream = value;

                    OnPropertyChanged();
                }
            }
        }
    }
}