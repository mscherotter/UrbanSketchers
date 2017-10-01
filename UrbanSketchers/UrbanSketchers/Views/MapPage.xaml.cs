using System;
using System.Linq;
using System.Threading.Tasks;
using UrbanSketchers.Data;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Views
{
    /// <summary>
    /// Map Page
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        private Sketch _sketch;

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

            this.Map.Pins.SetRange(pins);
        }

        private async void OnRefresh(object sender, EventArgs e)
        {
            await RefreshAsync();
        }

        private void OnAddSketch(object sender, EventArgs e)
        {
            this.EditSketchView.IsVisible = true;

            _sketch = new Sketch
            {
                CreationDate = DateTime.Now
            };

            UpdateLocation();

            this.EditSketchView.BindingContext = _sketch;
        }

        private async void OnSketchSaved(object sender, EventArgs e)
        {
            await RefreshAsync();

            _sketch = null;
        }

        private void OnMapPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Map.VisibleRegion))
            {
                if (_sketch != null)
                {
                    UpdateLocation();
                }
            }
        }

        private void UpdateLocation()
        {
            _sketch.Latitude = Map.VisibleRegion.Center.Latitude;
            _sketch.Longitude = Map.VisibleRegion.Center.Longitude;
        }

        private void OnSketchCanceled(object sender, EventArgs e)
        {
            _sketch = null;
        }
    }
}