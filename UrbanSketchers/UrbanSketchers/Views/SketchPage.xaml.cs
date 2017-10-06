using System;
using System.Threading.Tasks;
using UrbanSketchers.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SketchPage
    {
        public SketchPage()
        {
            InitializeComponent();
        }

        public string SketchId { get; internal set; }

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
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.UWP:
                        LikeButton.Icon = new FileImageSource { File = "Assets/FilledHeart.png" };
                        break;
                }
            }
            else
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.UWP:
                        LikeButton.Icon = new FileImageSource { File = "Assets/EmptyHeart.png" };
                        break;
                }
            }
        }

        private async void OnEdit(object sender, EventArgs e)
        {
            if (SketchManager.DefaultManager.CurrentClient.CurrentUser == null)
            {
                var authenticated = await App.Authenticator.AuthenticateAsync();

                if (!authenticated)
                {
                    return;
                }

                if (BindingContext is Sketch sketch)
                {
                    var currentUser = await SketchManager.DefaultManager.GetCurrentUserAsync();

                    if (sketch.CreatedBy != currentUser.Id)
                    {
                        await DisplayAlert("Edit Sketch", "You can only edit sketches that you submitted.", "OK");

                        return;
                    }
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
            if (BindingContext is Sketch sketch)
            {
                await Navigation.PushAsync(new PersonPage
                {
                    PersonId = sketch.CreatedBy
                });
            }
        }

        /// <summary>
        /// User likes or un-likes the photo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnLike(object sender, EventArgs e)
        {
            if (BindingContext is Sketch sketch)
            {
                LikeButton.IsEnabled = false;

                var rating = await SketchManager.DefaultManager.GetRatingAsync(sketch.Id);

                if (rating == null)
                {
                    rating = new Rating
                    {
                        IsHeart = true,
                        SketchId = sketch.Id,
                    };
                }
                else
                {
                    rating.IsHeart = !rating.IsHeart;
                }

                await SketchManager.DefaultManager.SaveAsync(rating);

                UpdateLikeButton(rating);

                LikeButton.IsEnabled = true;
            }
        }
    }
}
