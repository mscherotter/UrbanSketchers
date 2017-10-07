﻿using System;
using System.Globalization;
using System.Threading.Tasks;
using Plugin.Share;
using Plugin.Share.Abstractions;
using UrbanSketchers.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Views
{
    /// <summary>
    ///     Sketch page
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SketchPage
    {
        public SketchPage()
        {
            InitializeComponent();

            ShareItem.IsEnabled = CrossShare.IsSupported;
        }

        public string SketchId { get; internal set; }

        public Sketch Sketch => BindingContext as Sketch;

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            var sketch = await SketchManager.DefaultManager.GetSketchAsync(SketchId);

            if (sketch != null)
            {
                var rating = await SketchManager.DefaultManager.GetRatingAsync(sketch.Id);

                UpdateLikeButton(rating);
            }

            BindingContext = sketch;
        }

        private void UpdateLikeButton(Rating rating)
        {
            if (rating != null && rating.IsHeart)
                switch (Device.RuntimePlatform)
                {
                    case Device.UWP:
                        LikeButton.Icon = new FileImageSource {File = "Assets/FilledHeart.png"};
                        break;
                }
            else
                switch (Device.RuntimePlatform)
                {
                    case Device.UWP:
                        LikeButton.Icon = new FileImageSource {File = "Assets/EmptyHeart.png"};
                        break;
                }
        }

        private async void OnEdit(object sender, EventArgs e)
        {
            if (SketchManager.DefaultManager.CurrentClient.CurrentUser == null)
            {
                var authenticated = await App.Authenticator.AuthenticateAsync();

                if (!authenticated)
                    return;

                var currentUser = await SketchManager.DefaultManager.GetCurrentUserAsync();

                if (Sketch != null && Sketch.CreatedBy != currentUser.Id)
                {
                    await DisplayAlert(
                        Properties.Resources.EditSketch,
                        Properties.Resources.OnlyEditOwnSketches,
                        Properties.Resources.OK);

                    return;
                }
            }

            EditSketch.IsVisible = true;
        }

        private async void OnRefresh(object sender, EventArgs e)
        {
            await RefreshAsync();
        }

        private async void OnTappedName(object sender, EventArgs e)
        {
            if (Sketch == null) return;

            await Navigation.PushAsync(new PersonPage
            {
                PersonId = Sketch.CreatedBy
            });
        }

        /// <summary>
        ///     User likes or un-likes the photo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnLike(object sender, EventArgs e)
        {
            if (Sketch == null) return;

            LikeButton.IsEnabled = false;

            var rating = await SketchManager.DefaultManager.GetRatingAsync(Sketch.Id);

            if (rating == null)
                rating = new Rating
                {
                    IsHeart = true,
                    SketchId = Sketch.Id
                };
            else
                rating.IsHeart = !rating.IsHeart;

            await SketchManager.DefaultManager.SaveAsync(rating);

            UpdateLikeButton(rating);

            LikeButton.IsEnabled = true;
        }

        private async void OnDelete(object sender, EventArgs e)
        {
            if (Sketch == null) return;

            var person = await SketchManager.DefaultManager.GetCurrentUserAsync();

            if (person == null)
                if (await App.Authenticator.AuthenticateAsync())
                    person = await SketchManager.DefaultManager.GetCurrentUserAsync();

            if (person == null)
                return;

            if (person.Id == Sketch.CreatedBy)
            {
                await SketchManager.DefaultManager.DeleteAsync(Sketch);

                await Navigation.PopAsync(true);
            }
        }

        private void OnShare(object sender, EventArgs e)
        {
            if (Sketch == null) return;

            var message = new ShareMessage
            {
                Title = string.Format(
                    CultureInfo.CurrentCulture,
                    Properties.Resources.TitleBySketcher,
                    Sketch.Title,
                    Sketch.CreatedByName),
                Url = Sketch.ImageUrl
            };

            CrossShare.Current.Share(message);
        }
    }
}