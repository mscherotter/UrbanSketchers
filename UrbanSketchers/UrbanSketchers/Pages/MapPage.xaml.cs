#define SKETCH_MAP
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.WindowsAzure.MobileServices;
using UrbanSketchers.Controls;
using UrbanSketchers.Data;
using UrbanSketchers.Interfaces;
using UrbanSketchers.Support;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
using Container = UrbanSketchers.Core.Container;

namespace UrbanSketchers.Pages
{
    /// <summary>
    ///     Map Page
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : IMapPage
    {
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
                await EditSketchView.LoadImageStreamAsync(_imageStream);

                _imageStream.Dispose();

                _imageStream = null;

                OnAddSketch(null, null);
            }

            try
            {
                var user = await Container.Current.Resolve<ISketchManager>().GetCurrentUserAsync();

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

        private void SearchCompleted(object sender, EventArgs e)
        {
            if (sender is Entry entry)
            {
                var locations = BingSearch.LocationSearch(entry.Text);

                if (locations == null) return;

                SearchResults.ItemsSource = locations;
                SearchResults.IsVisible = true;

                SearchResults.Focus();
            }
        }

        private void OnSearchResultSelected(object sender, EventArgs e)
        {
            if (SearchResults.SelectedItem is Resource resource && resource.Point?.Coordinates != null && resource.Point.Coordinates.Length == 2)
            {
                var position = new Position(resource.Point.Coordinates[0], resource.Point.Coordinates[1]);

                var span = MapSpan.FromCenterAndRadius(position,
                    Distance.FromMiles(1.0));

                Map.MoveToRegion(span);
            }
        }

        #region Fields

        //private string _personId;
        private ISketch _sketch;

        private MemoryStream _imageStream;
        private MemoryStream _inkStream;

        #endregion

        #region Implementation

        private Task<bool> ShouldUpload(ISketch arg)
        {
            return DisplayAlert(
                Properties.Resources.UploadSketch,
                Properties.Resources.UploadMessage,
                Properties.Resources.OK,
                Properties.Resources.Cancel);
        }

        private static ISketchPin CreatePin(ISketch sketch)
        {
            var pin = Container.Current.Resolve<ISketchPin>();

            pin.Pin = new Pin
            {
                Type = PinType.Place,
                Position = new Position(sketch.Latitude, sketch.Longitude),
                Label = sketch.Title,
                Id = sketch.Id,
                Address = sketch.Address
            };

            pin.Url = sketch.ThumbnailUrl;

            return pin;
        }

        private async Task RefreshAsync()
        {
            try
            {
                if (Map.VisibleRegion == null) return;

                var sector = CustomIndexing.LatLonToSector(Map.VisibleRegion.Center.Latitude,
                    Map.VisibleRegion.Center.Longitude,
                    CustomIndexing.SectorSize);


                var sectorArea = CustomIndexing.Area(sector, CustomIndexing.SectorSize);

                var radius = Map.VisibleRegion.Radius.Kilometers;

                var visibleArea = Math.Pow(radius * 2.0, 2.0);

                Debug.WriteLine($"Sector area: {sectorArea}, Visible area: {visibleArea}.");

                var sketches = visibleArea > sectorArea
                    ? await Container.Current.Resolve<ISketchManager>().GetSketchsAsync()
                    : await Container.Current.Resolve<ISketchManager>().GetSketchsAsync(sector);

                if (sketches == null)
                    return;

                if (sketches is MobileServiceCollection<Sketch> collection)
                    Title = string.Format(CultureInfo.CurrentCulture, Properties.Resources.SketchMapNSketches,
                        collection.TotalCount);

#if SKETCH_MAP
                var pins = from sketch in sketches
                    select CreatePin(sketch);

                var pinList = pins.ToList();

                foreach (var pin in pinList)
                    pin.Clicked += Pin_Clicked;

                Map.CustomPins.SetRange(pinList);
#else
                var pins = from sketch in sketches
                    select new Pin
                    {
                        Position = new Position(sketch.Latitude, sketch.Longitude),
                        Type = PinType.Place,
                        Label = sketch.Title,
                        Id = sketch.Id
                    };

                var pinList = pins.ToList();

                foreach (var pin in pinList)
                    pin.Clicked += Pin_Clicked;

                //Map.CustomPins.SetRange(pinList);

                Map.Pins.SetRange(pinList);
#endif
            }
            catch (Exception e)
            {
                Debug.WriteLine("Map page refresh failed: {0}", e.Message);
            }
        }

        private async void Pin_Clicked(object sender, EventArgs e)
        {
            switch (sender)
            {
                case ISketchPin pin:
                {
                    var page = Container.Current.Resolve<ISketchPage>();

                    page.SketchId = pin.Pin.Id.ToString();

                    await Navigation.PushAsync(page as Page, false);
                    break;
                }
                case Pin pin2:
                {
                    var page = Container.Current.Resolve<ISketchPage>();

                    page.SketchId = pin2.Id.ToString();

                    await Navigation.PushAsync(page as Page, false);
                    break;
                }
            }
        }

        private async void OnRefresh(object sender, EventArgs e)
        {
            await RefreshAsync();
        }

        private async void OnAddSketch(object sender, EventArgs e)
        {
            var editSketchPage = Container.Current.Resolve<IEditSketchPage>();

            _sketch = Container.Current.Resolve<ISketch>();

            _sketch.Latitude = Map.VisibleRegion.Center.Latitude;
            _sketch.Longitude = Map.VisibleRegion.Center.Longitude;
            editSketchPage.Radius = Map.VisibleRegion.Radius;

            editSketchPage.MapType = Map.MapType;

            editSketchPage.Sketch = _sketch;

            await Navigation.PushModalAsync(editSketchPage as Page, true);
            //page.SketchId

            //if (SketchManager.DefaultManager.CurrentClient.CurrentUser == null)
            //{
            //    var authenticated = await App.Authenticator.AuthenticateAsync();

            //    if (!authenticated)
            //        return;
            //}

            ////if (string.IsNullOrWhiteSpace(_personId))
            ////{
            ////    var table = SketchManager.DefaultManager.CurrentClient.GetTable<Person>();

            ////    var query = from item in table
            ////        where item.UserId == SketchManager.DefaultManager.CurrentClient.CurrentUser.UserId
            ////        select item;

            ////    var results = await query.ToEnumerableAsync();

            ////    var person = results.FirstOrDefault();

            ////    if (person == null)
            ////    {
            ////        person = new Person
            ////        {
            ////            UserId = SketchManager.DefaultManager.CurrentClient.CurrentUser.UserId
            ////        };

            ////        await SketchManager.DefaultManager.SaveAsync(person);

            ////        _personId = person.Id;
            ////    }
            ////    else
            ////    {
            ////        _personId = person.Id;
            ////    }
            ////}

            //EditSketchView.IsVisible = true;
            //Crosshair.IsVisible = true;

            //_sketch = DependencyService.Get<ISketch>(DependencyFetchTarget.NewInstance);

            //await UpdateLocationAsync();

            //EditSketchView.BindingContext = _sketch;
        }

        private async void OnSketchSaved(object sender, TypedEventArgs<ISketch> e)
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
                _inkStream = new MemoryStream();

            var page = Container.Current.Resolve<IDrawingPage>();

            page.ImageStream = _imageStream;
            page.InkStream = _inkStream;

            await Navigation.PushModalAsync(page as Page, true);
        }

        private async void OnSelectMapType(object sender, EventArgs args)
        {
            var action = await DisplayActionSheet(
                "Map type",
                Properties.Resources.Cancel,
                null,
                MapType.Satellite.ToString(),
                MapType.Street.ToString(),
                MapType.Hybrid.ToString());

            if (string.IsNullOrWhiteSpace(action)) return;

            Map.MapType = (MapType) Enum.Parse(typeof(MapType), action);
        }

        #endregion
    }
}