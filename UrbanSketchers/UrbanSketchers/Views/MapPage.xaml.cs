using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using UrbanSketchers.Controls;
using UrbanSketchers.Data;
using UrbanSketchers.Support;
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
        #region Fields

        //private string _personId;
        private Sketch _sketch;
        private MemoryStream _imageStream;
        private MemoryStream _inkStream;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the MapPage class.
        /// </summary>
        public MapPage()
        {
            InitializeComponent();

            EditSketchView.ShouldUpload = ShouldUpload;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Refresh the map pins when it appears
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_imageStream != null && _imageStream.Length > 0)
            {
                EditSketchView.LoadImageStream(_imageStream);

                _imageStream.Dispose();
                
                _imageStream = null;

                OnAddSketch(null, null);
            }

            try
            {
                var user = await SketchManager.DefaultManager.GetCurrentUserAsync();

                if (user != null)
                {
                }

                await RefreshAsync();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        #endregion

        #region Implementation

        private Task<bool> ShouldUpload(Sketch arg)
        {
            return DisplayAlert(
                Properties.Resources.UploadSketch,
                Properties.Resources.UploadMessage,
                Properties.Resources.OK,
                Properties.Resources.Cancel);
        }

        private async Task RefreshAsync()
        {
            if (Map.VisibleRegion == null) return;

            var sector = CustomIndexing.LatLonToSector(Map.VisibleRegion.Center.Latitude, Map.VisibleRegion.Center.Longitude,
                CustomIndexing.SectorSize);


            var sectorArea = CustomIndexing.Area(sector, CustomIndexing.SectorSize);

            var radius = Map.VisibleRegion.Radius.Kilometers;

            var visibleArea = Math.Pow(radius * 2.0, 2.0);

            MobileServiceCollection<Sketch, Sketch> sketches;

            System.Diagnostics.Debug.WriteLine($"Sector area: {sectorArea}, Visible area: {visibleArea}.");

            if (visibleArea > sectorArea)
            {
                sketches = await SketchManager.DefaultManager.GetSketchsAsync();
            }
            else
            {
                sketches = await SketchManager.DefaultManager.GetSketchsAsync(sector);
            }

            if (sketches == null)
                return;

            Title = string.Format(CultureInfo.CurrentCulture, Properties.Resources.SketchMapNSketches,
                sketches.TotalCount);

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
                }, false);
        }

        private async void OnRefresh(object sender, EventArgs e)
        {
            await RefreshAsync();
        }

        private async void OnAddSketch(object sender, EventArgs e)
        {
            if (SketchManager.DefaultManager.CurrentClient.CurrentUser == null)
            {
                var authenticated = await App.Authenticator.Authenticate();

                if (!authenticated)
                    return;
            }

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

            if (_inkStream != null)
            {
                _inkStream.Dispose();

                _inkStream = null;
            }

            _sketch = null;
        }

        private async void OnMapPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Map.VisibleRegion))
                await UpdateLocationAsync();
        }

        private async Task UpdateLocationAsync()
        {
            if (_sketch != null)
            {
                _sketch.Latitude = Map.VisibleRegion.Center.Latitude;
                _sketch.Longitude = Map.VisibleRegion.Center.Longitude;
            }

            await RefreshAsync();
        }

        private void OnSketchCanceled(object sender, EventArgs e)
        {
            Crosshair.IsVisible = false;

            _sketch = null;
        }

        private async void OnDrawSketch(object sender, EventArgs e)
        {
            _imageStream = new MemoryStream();

            if (_inkStream == null)
            {
                _inkStream = new MemoryStream();
            }

            await Navigation.PushModalAsync(new DrawingPage
            {
                ImageStream = _imageStream,
                InkStream = _inkStream
            }, true);
        }
        #endregion
    }
}