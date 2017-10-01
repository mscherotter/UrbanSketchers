using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        public MapPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            var sketches = await SketchManager.DefaultManager.GetSketchsAsync();

            var pins = from sketch in sketches
                select new Pin
                {
                    Type = PinType.Place,
                    Position = new Position(sketch.Latitude, sketch.Longitude),
                    Label = sketch.Title,
                    Id = sketch.Id,
                    Address = sketch.Address
                };

            this.Map.Pins.Clear();

            foreach (var item in pins)
            {
                this.Map.Pins.Add(item);
            }
        }
    }
}