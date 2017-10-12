using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrbanSketchers.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DrawingPage : ContentPage
    {
        public DrawingPage()
        {
            InitializeComponent();
        }

        public Stream ImageStream { get; set; }

        private async void OnAccept(object sender, EventArgs e)
        {
            if (DrawingCanvas.GetImageFunc != null)
            {
                await DrawingCanvas.GetImageFunc(ImageStream, DrawingCanvas.BitmapFileFormat.Png);
            }

            await Navigation.PopModalAsync(true);
        }

        private async void OnCancel(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(true);
        }
    }
}