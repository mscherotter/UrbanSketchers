﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using UrbanSketchers.Data;
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
        private string _personId;
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
                    Address = sketch.Address,
                };

            var pinList = pins.ToList();

            foreach (var pin in pinList)
            {
                pin.Clicked += Pin_Clicked;
            }

            Map.Pins.SetRange(pinList);
        }

        private async void Pin_Clicked(object sender, EventArgs e)
        {
            if (sender is Pin pin)
            {
                await Navigation.PushAsync(new SketchPage
                {
                    SketchId = pin.Id.ToString()
                });
            }
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

            if (string.IsNullOrWhiteSpace(_personId))
            {
                var table = SketchManager.DefaultManager.CurrentClient.GetTable<Person>();

                var query = from item in table
                    where item.UserId == SketchManager.DefaultManager.CurrentClient.CurrentUser.UserId
                    select item;

                var results = await query.ToEnumerableAsync();

                var person = results.FirstOrDefault();

                if (person == null)
                {
                    //using (var client = new HttpClient())
                    //{
                    //    var userId = SketchManager.DefaultManager.CurrentClient.CurrentUser.UserId;
                    //    var accessToken = SketchManager.DefaultManager.CurrentClient.CurrentUser.MobileServiceAuthenticationToken;
                    //    var facebookId = userId.Substring(userId.IndexOf(':') + 1);

                    //    var requestUri = string.Format(
                    //        "https://graph.facebook.com/v2.5/me?access_token={0}", 
                    //        accessToken);

                    //    var response = await client.GetStringAsync(requestUri);


                    //}
                    person = new Person
                    {
                        UserId = SketchManager.DefaultManager.CurrentClient.CurrentUser.UserId
                    };

                    await SketchManager.DefaultManager.SaveAsync(person);

                    _personId = person.Id;
                }
                else
                {
                    _personId = person.Id;
                }
            }

            EditSketchView.IsVisible = true;

            _sketch = new Sketch
            {
                CreationDate = DateTime.Now,
                CreatedBy = _personId
            };

            UpdateLocation();

            EditSketchView.BindingContext = _sketch;
        }

        private async void OnSketchSaved(object sender, EventArgs e)
        {
            await RefreshAsync();

            _sketch = null;
        }

        private void OnMapPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Map.VisibleRegion))
                if (_sketch != null)
                    UpdateLocation();
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

        private void OnLogin(object sender, EventArgs e)
        {
        }
    }
}