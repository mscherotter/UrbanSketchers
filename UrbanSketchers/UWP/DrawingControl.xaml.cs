using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Microsoft.Graphics.Canvas;
using UrbanSketchers.Controls;

namespace UWP
{
    /// <summary>
    ///     Drawing control with an InkCanvas and InkToolbar
    /// </summary>
    public sealed partial class DrawingControl
    {
        /// <summary>
        /// Initializes a new instance of the DrawingControl class.
        /// </summary>
        public DrawingControl()
        {
            InitializeComponent();
        }

        internal async Task<bool> GetImageAsync(Stream stream, DrawingCanvas.BitmapFileFormat format)
        {
            if (stream == null)
            {
                return false;
            }

            var strokes = DrawingCanvas.InkPresenter.StrokeContainer.GetStrokes();

            if (strokes == null)
                return false;

            if (!strokes.Any())
                return false;

            var device = CanvasDevice.GetSharedDevice();

            using (var offscreen = new CanvasRenderTarget(
                device,
                Convert.ToSingle(DrawingCanvas.Width),
                Convert.ToSingle(DrawingCanvas.Height),
                96))
            {
                using (var ds = offscreen.CreateDrawingSession())
                {
                    if (Background is SolidColorBrush brush)
                    {
                        ds.Clear(brush.Color);    
                    }
                    else
                    {
                        ds.Clear(Colors.White);
                    }

                    ds.DrawInk(strokes);
                }

                if (!Enum.TryParse(format.ToString(), out CanvasBitmapFileFormat fileFormat))
                    return false;

                await offscreen.SaveAsync(stream.AsRandomAccessStream(), fileFormat);

                stream.Seek(0, SeekOrigin.Begin);

                return true;
            }
        }
    }
}