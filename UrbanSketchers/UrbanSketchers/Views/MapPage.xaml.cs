using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using UrbanSketchers.Controls;
using UrbanSketchers.Data;
using UrbanSketchers.Services;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Views
{
    /// <summary>
    ///     Map Page
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage
    {
        private bool _authenticated;

        //private string _personId;
        private Sketch _sketch;

        /// <summary>
        /// Initializes a new instance of the MapPage class.
        /// </summary>
        public MapPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Refresh the map pins when it appears
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var user = await SketchManager.DefaultManager.GetCurrentUserAsync();

            if (user != null)
            {
            }

            await RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            if (Map.VisibleRegion == null) return;

            //var sector = CustomIndexing.LatLonToSector(Map.VisibleRegion.Center.Latitude, Map.VisibleRegion.Center.Longitude,
            //    CustomIndexing.SectorSize);

            var sketches = await SketchManager.DefaultManager.GetSketchsAsync();

            var pins = from sketch in sketches
                select new SketchPin
                {
                    Pin = new Pin
                    {
                        Type = PinType.Place,
                        Position = new Position(sketch.Latitude, sketch.Longitude),
                        Label = sketch.Title,
                        Id = sketch.Id,
                        Address = sketch.Address
                    },
                    Url = sketch.ThumbnailUrl
                };

            var pinList = pins.ToList();

            foreach (var pin in pinList)
                pin.Clicked += Pin_Clicked;

            Map.CustomPins.SetRange(pinList);

            //Map.Pins.SetRange(from pin in pinList
            //    select pin.Pin);
        }

        private async void Pin_Clicked(object sender, EventArgs e)
        {
            if (sender is SketchPin pin)
                await Navigation.PushAsync(new SketchPage
                {
                    SketchId = pin.Pin.Id.ToString()
                });
        }

        private async void OnRefresh(object sender, EventArgs e)
        {
            await RefreshAsync();
        }

        private async void OnAddSketch(object sender, EventArgs e)
        {
            if (!_authenticated)
                _authenticated = await App.Authenticator.AuthenticateAsync();

            if (!_authenticated) return;

            //if (string.IsNullOrWhiteSpace(_personId))
            //{
            //    var table = SketchManager.DefaultManager.CurrentClient.GetTable<Person>();

            //    var query = from item in table
            //        where item.UserId == SketchManager.DefaultManager.CurrentClient.CurrentUser.UserId
            //        select item;

            //    var results = await query.ToEnumerableAsync();

            //    var person = results.FirstOrDefault();

            //    if (person == null)
            //    {
            //        person = new Person
            //        {
            //            UserId = SketchManager.DefaultManager.CurrentClient.CurrentUser.UserId
            //        };

            //        await SketchManager.DefaultManager.SaveAsync(person);

            //        _personId = person.Id;
            //    }
            //    else
            //    {
            //        _personId = person.Id;
            //    }
            //}

            EditSketchView.IsVisible = true;
            Crosshair.IsVisible = true;

            _sketch = new Sketch
            {
                CreationDate = DateTime.Now
            };

            await UpdateLocationAsync();

            EditSketchView.BindingContext = _sketch;
        }

        private async void OnSketchSaved(object sender, TypedEventArgs<Sketch> e)
        {
            Crosshair.IsVisible = false;

            await RefreshAsync();

            _sketch = null;
        }

        private async void OnMapPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Map.VisibleRegion))
                if (_sketch != null)
                    await UpdateLocationAsync();
        }

        private async Task UpdateLocationAsync()
        {
            _sketch.Latitude = Map.VisibleRegion.Center.Latitude;
            _sketch.Longitude = Map.VisibleRegion.Center.Longitude;

            await RefreshAsync();
        }

        private void OnSketchCanceled(object sender, EventArgs e)
        {
            Crosshair.IsVisible = false;

            _sketch = null;
        }

        private void OnLogin(object sender, EventArgs e)
        {
        }
    }
}