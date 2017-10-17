﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using UrbanSketchers;
using UrbanSketchers.Data;

namespace UWP
{
    /// <summary>
    ///     Share target page
    /// </summary>
    public sealed partial class ShareTargetPage
    {
        private readonly MobileServiceInit _mobileServiceInit;
        private readonly Sketch _sketch;
        private MemoryStream _imageStream;
        private bool _isAdding;
        private ShareOperation _shareOperation;

        /// <summary>
        /// Initializes a new instance of the ShareTargetPage class.
        /// </summary>
        public ShareTargetPage()
        {
            InitializeComponent();

            _sketch = new Sketch
            {
                CreationDate = DateTime.UtcNow
            };

            DataContext = _sketch;

            _mobileServiceInit = new MobileServiceInit();

            _mobileServiceInit.SignedIn += _mobileServiceInit_SignedIn;
        }

        private async void _mobileServiceInit_SignedIn(object sender, EventArgs e)
        {
            if (_isAdding)
                await AddAsync();
        }

        /// <summary>
        /// Navigate to the page
        /// </summary>
        /// <param name="e">the navigation event arguments</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            _shareOperation = e.Parameter as ShareOperation;
            if (_shareOperation != null)
            {
                _sketch.Title = _shareOperation.Data.Properties.Title;
                _sketch.Description = _shareOperation.Data.Properties.Description;
                if (_shareOperation.Data.Properties.TryGetValue("Address", out object value))
                    _sketch.Address = value.ToString();

                if (_shareOperation.Data.Contains(StandardDataFormats.Bitmap))
                {
                    var streamReference = await _shareOperation.Data.GetBitmapAsync();

                    _imageStream = new MemoryStream();

                    var bitmapStream = await streamReference.OpenReadAsync();

                    var sourceStream = bitmapStream.AsStreamForRead();

                    await sourceStream.CopyToAsync(_imageStream);

                    _imageStream.Seek(0, SeekOrigin.Begin);

                    var image = new BitmapImage();

                    bitmapStream.Seek(0);

                    await image.SetSourceAsync(bitmapStream);

                    Image.Source = image;
                }
                else if (_shareOperation.Data.Contains(StandardDataFormats.StorageItems))
                {
                    var items = await _shareOperation.Data.GetStorageItemsAsync();

                    var file = (from item in items.OfType<StorageFile>()
                        where item.ContentType.StartsWith("image/")
                        select item).FirstOrDefault();

                    if (file != null)
                    {
                        _imageStream = new MemoryStream();

                        using (var bitmapStream = await file.OpenStreamForReadAsync())
                        {
                            await bitmapStream.CopyToAsync(_imageStream);

                            _imageStream.Seek(0, SeekOrigin.Begin);

                            var image = new BitmapImage();

                            bitmapStream.Seek(0, SeekOrigin.Begin);

                            await image.SetSourceAsync(bitmapStream.AsRandomAccessStream());

                            Image.Source = image;
                        }
                    }
                }
            }

            if (_imageStream == null)
                AddButton.IsEnabled = false;

            base.OnNavigatedTo(e);
        }

        private void OnCenterChanged(MapControl sender, object args)
        {
            _sketch.Latitude = Map.Center.Position.Latitude;
            _sketch.Longitude = Map.Center.Position.Longitude;
        }

        private async void OnAdd(object sender, RoutedEventArgs e)
        {
            if (!_mobileServiceInit.Authenticate())
            {
                _isAdding = true;

                return;
            }

            await AddAsync();
        }

        private async Task AddAsync()
        {
            ProgressRing.IsActive = true;

            AddButton.IsEnabled = false;

            ProgressBar.Visibility = Visibility.Visible;

            ProgressBar.Value = 0;

            try
            {
                ProgressBar.Value = 33;

                var filename = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}{1}",
                    Guid.NewGuid().ToString(),
                    ".bmp");

                _sketch.ImageUrl = await SketchManager.DefaultManager.UploadAsync(filename, _imageStream);

                ProgressBar.Value = 66;

                await SketchManager.DefaultManager.SaveAsync(_sketch);

                ProgressBar.Value = 100;

                _shareOperation.ReportCompleted();
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Error sharing to this app: {exception.Message}");
            }

            ProgressRing.IsActive = false;
            ProgressBar.Visibility = Visibility.Collapsed;

            AddButton.IsEnabled = true;
        }

        private async void OnLocateMe(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button != null) button.IsEnabled = false;

            var status = await Geolocator.RequestAccessAsync();

            if (status == GeolocationAccessStatus.Allowed)
            {
                var locator = new Geolocator();

                var position = await locator.GetGeopositionAsync();

                await Map.TrySetViewAsync(new Geopoint(new BasicGeoposition
                {
                    Latitude = position.Coordinate.Point.Position.Latitude,
                    Longitude = position.Coordinate.Point.Position.Longitude
                }), 17.0);
            }

            if (button != null) button.IsEnabled = true;
        }
    }
}