using System;
using System.IO;
using UrbanSketchers.Controls;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Views
{
    /// <summary>
    ///     Drawing Page
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DrawingPage
    {
        /// <summary>
        ///     Initializes a new instance of the DrawingPage class.
        /// </summary>
        public DrawingPage()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Gets or sets the image stream
        /// </summary>
        public Stream ImageStream { get; set; }

        /// <summary>
        /// Gets or sets the ink stream
        /// </summary>
        public MemoryStream InkStream { get; set; }

        /// <summary>
        /// update the drawing canvas when the page appears
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            DrawingCanvas.InkStream = InkStream;
        }

        /// <summary>
        ///     Fills the image stream with the sketch and pops the modal navigation
        /// </summary>
        /// <param name="sender">the button</param>
        /// <param name="e">the event arguments</param>
        private async void OnAccept(object sender, EventArgs e)
        {
            if (DrawingCanvas.GetImageFunc != null)
            {
                await DrawingCanvas.GetImageFunc(ImageStream, DrawingFileFormat.Png);

                await DrawingCanvas.GetImageFunc(InkStream, DrawingFileFormat.Isf);
            }

            await Navigation.PopModalAsync(true);
        }

        /// <summary>
        ///     Pop the modal navigation stack
        /// </summary>
        /// <param name="sender">the button</param>
        /// <param name="e">the event arguments</param>
        private async void OnCancel(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(true);
        }
    }
}