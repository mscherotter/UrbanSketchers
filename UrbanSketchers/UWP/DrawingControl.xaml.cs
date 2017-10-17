using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
        ///     Initializes a new instance of the DrawingControl class.
        /// </summary>
        public DrawingControl()
        {
            InitializeComponent();
        }

        internal async Task LoadAsync(Stream stream)
        {
            if (stream == null || stream.Length == 0)
                return;

            stream.Seek(0, SeekOrigin.Begin);

            await DrawingCanvas.InkPresenter.StrokeContainer.LoadAsync(stream.AsInputStream());
        }

        /// <summary>
        ///     Get the ink image
        /// </summary>
        /// <param name="stream">the stream [in,out]</param>
        /// <param name="format">the format</param>
        /// <returns>an async task with a boolean value indicating success</returns>
        internal async Task<bool> GetImageAsync(Stream stream, DrawingFileFormat format)
        {
            if (stream == null)
                return false;

            stream.Seek(0, SeekOrigin.Begin);

            switch (format)
            {
                case DrawingFileFormat.Isf:
                    await DrawingCanvas.InkPresenter.StrokeContainer.SaveAsync(stream.AsOutputStream(),
                        InkPersistenceFormat.Isf);
                    return true;

                case DrawingFileFormat.GifWithEmbeddedIsf:
                    await DrawingCanvas.InkPresenter.StrokeContainer.SaveAsync(stream.AsOutputStream(),
                        InkPersistenceFormat.GifWithEmbeddedIsf);
                    return true;
            }

            return await DrawImageAsync(stream, format);
        }

        private async Task<bool> DrawImageAsync(Stream stream, DrawingFileFormat format)
        {
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
                        ds.Clear(brush.Color);
                    else
                        ds.Clear(Colors.White);

                    ds.DrawInk(strokes);
                }

                if (!Enum.TryParse(format.ToString(), true, out CanvasBitmapFileFormat fileFormat))
                    return false;

                await offscreen.SaveAsync(stream.AsRandomAccessStream(), fileFormat);

                stream.Seek(0, SeekOrigin.Begin);

                return true;
            }
        }

        private void OnBackgroundColorClicked(object sender, RoutedEventArgs e)
        {
            Control picker;

            var color = Colors.White;

            if (Background is SolidColorBrush brush)
                color = brush.Color;

            if (ApiInformation.IsTypePresent("Windows.UI.Xaml.Controls.ColorPicker"))
            {
                var newPicker = new ColorPicker
                {
                    Color = color,
                    Width = 300,
                    Height = 300
                };

                newPicker.ColorChanged += NewPicker_ColorChanged;
                picker = newPicker;
            }
            else
            {
                var newPicker = new Coding4Fun.Toolkit.Controls.ColorPicker
                {
                    Color = color,
                    Width = 300,
                    Height = 300
                };

                newPicker.ColorChanged += NewPicker_ColorChanged1;
                picker = newPicker;
            }

            BackgroundColorButton.ContextFlyout = new Flyout
            {
                Content = picker
            };

            BackgroundColorButton.ContextFlyout.ShowAt(BackgroundColorButton);
        }

        private void NewPicker_ColorChanged1(object sender, Color color)
        {
            Background = new SolidColorBrush(color);
        }

        private void NewPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            Background = new SolidColorBrush(args.NewColor);
        }
    }
}