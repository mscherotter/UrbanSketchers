﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media.Animation;
using Windows.Web.Http;
using UrbanSketchers.Controls;
using UWP;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.UWP;
using Xamarin.Forms.Platform.UWP;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

[assembly: ExportRenderer(typeof(SketchMap), typeof(SketchMapRenderer))]

namespace UWP
{
    public class SketchMapRenderer : MapRenderer
    {
        private MapControl _nativeMap;
        private Canvas _sourceCanvas;
        private ObservableCollection<SketchPin> _pins;
        private Image _sourceImage;

        protected override async void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                _pins.CollectionChanged -= _pins_CollectionChanged;

                _nativeMap.MapElementClick -= OnMapElementClick;
                _nativeMap.Children.Clear();
                _nativeMap = null;
            }

            if (e.NewElement != null)
            {
                if (e.NewElement is SketchMap sketchMap)
                {
                    _pins = sketchMap.CustomPins;
                    _pins.CollectionChanged += _pins_CollectionChanged;
                }

                _nativeMap = Control;

                _sourceCanvas = new Canvas();

                _sourceImage = new Image
                {
                    IsHitTestVisible = false,
                    //Visibility = Visibility.Collapsed
                };

                _sourceCanvas.Children.Add(_sourceImage);

                _nativeMap.Children.Add(_sourceCanvas);

                _nativeMap.MapElementClick += OnMapElementClick;

                UpdatePinsAsync();
            }
        }

        private void _pins_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    //_nativeMap.Children.Clear();
                    _nativeMap.MapElements.Clear();
                    break;
                case NotifyCollectionChangedAction.Add:
                    UpdatePinsAsync();
                    break;
            }
        }

        private async Task<RandomAccessStreamReference> ResizeImageAsync(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
            {
                return null;
            }

            if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            {
                return null;
            }

            double maxSize = 300;

            if (Element is SketchMap sketchMap)
                maxSize = sketchMap.MaxImageSize;

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(new Uri(uri)))
                {
                    using (var inputStream = new InMemoryRandomAccessStream())
                    {
                        await response.Content.WriteToStreamAsync(inputStream.GetOutputStreamAt(0));

                        var imageStream = inputStream.AsStreamForRead();

                        try
                        {
                            var decoder = await BitmapDecoder.CreateAsync(imageStream.AsRandomAccessStream());

                        
                            var size = Math.Max(decoder.PixelHeight, decoder.PixelWidth);

                            var scale = maxSize / Convert.ToDouble(size);

                            var memStream = new InMemoryRandomAccessStream();

                            var encoder = await BitmapEncoder.CreateForTranscodingAsync(memStream, decoder);

                            encoder.BitmapTransform.ScaledWidth = Convert.ToUInt32(decoder.PixelWidth * scale);
                            encoder.BitmapTransform.ScaledHeight = Convert.ToUInt32(decoder.PixelHeight * scale);

                            await encoder.FlushAsync();

                            return RandomAccessStreamReference.CreateFromStream(memStream);
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine($"Could not decode {uri}: {e.Message}.");

                            return null;
                        }
                    }
                }
            }
        }

        private void UpdatePinsAsync()
        {
            //_nativeMap.Children.Clear();

            var pins = _pins.ToList();

            foreach (var pin in pins)
            {
                var position = new BasicGeoposition
                {
                    Latitude = pin.Pin.Position.Latitude,
                    Longitude = pin.Pin.Position.Longitude
                };

                var point = new Geopoint(position);

                var pinImage = string.IsNullOrWhiteSpace(pin.Url) ? null : RandomAccessStreamReference.CreateFromUri(new Uri(pin.Url));

                var mapIcon = new MapIcon
                {
                    Image = pinImage, //await ResizeImageAsync(pin.Url),
                    CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible,
                    Location = point,
                    NormalizedAnchorPoint = new Point(0.5, 1.0),
                    Title = pin.Pin.Label
                };

                MapIconProperties.SetSketchPin(mapIcon, pin);

                _nativeMap.MapElements.Add(mapIcon);
            }
        }

        private async void OnMapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            var icon = args.MapElements.OfType<MapIcon>().FirstOrDefault();

            if (icon != null)
            {
                var sketchPin = MapIconProperties.GetSketchPin(icon);

                if (ApiInformation.IsTypePresent("Windows.UI.Xaml.Media.Animation.ConnectedAnimationService"))
                {
                    var bitmapImage = new BitmapImage();

                    await bitmapImage.SetSourceAsync(await icon.Image.OpenReadAsync());

                    _sourceImage.Visibility = Visibility.Visible;
                    
                    _sourceImage.Source = bitmapImage;

                    Point point;

                    _nativeMap.GetOffsetFromLocation(icon.Location, out point);

                    Canvas.SetLeft(_sourceImage, point.X - bitmapImage.PixelWidth / 2.0);

                    Canvas.SetTop(_sourceImage, point.Y - bitmapImage.PixelHeight);
                    
                    ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("image", _sourceImage);

                    sketchPin.InvokeClick();

                    await Control.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async delegate
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1));

                        _sourceImage.Visibility = Visibility.Collapsed;
                    });
                }
                else
                {
                    sketchPin.InvokeClick();
                }
            }
        }
    }
}