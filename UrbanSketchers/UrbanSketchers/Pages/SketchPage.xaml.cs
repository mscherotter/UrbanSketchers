using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using Plugin.Share;
using Plugin.Share.Abstractions;
using UrbanSketchers.Data;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Pages
{
    /// <summary>
    ///     Sketch page
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SketchPage : ISketchPage
    {
        #region Fields

        #endregion

        /// <summary>
        ///     Initializes a new instance of the SketchPage class.
        /// </summary>
        public SketchPage()
        {
            InitializeComponent();

            ShareItem.IsEnabled = CrossShare.IsSupported;

            Image.PropertyChanged += Image_PropertyChanged;
        }

        /// <summary>
        ///     Gets the <see cref="Sketch" /> as the binding context
        /// </summary>
        public ISketch Sketch => BindingContext as ISketch;

        /// <summary>
        ///     Gets the sketch Id
        /// </summary>
        public string SketchId { get; set; }

        /// <summary>
        ///     Size changed - handle device orientation changes
        /// </summary>
        /// <param name="width">the new width</param>
        /// <param name="height">the new height</param>
        protected override void OnSizeAllocated(double width, double height)
        {
            if (width > height)
            {
                Grid.SetColumnSpan(Image, 1);
                Grid.SetRowSpan(Image, 2);
                Grid.SetRow(Map, 1);
                Grid.SetColumn(Map, 1);
                Grid.SetColumnSpan(Map, 1);
                Grid.SetRowSpan(Map, 2);
            }
            else
            {
                Grid.SetColumnSpan(Image, 2);
                Grid.SetRowSpan(Image, 1);
                Grid.SetRow(Map, 2);
                Grid.SetColumn(Map, 0);
                Grid.SetColumnSpan(Map, 2);
                Grid.SetRowSpan(Map, 1);
            }

            base.OnSizeAllocated(width, height);
        }

        private void Image_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Renderer") return;

            if (!Image.IsLoading && Image.Width > 0 && Image.Height > 0 && Image.Control != null &&
                Image.Source != null)
                Image.StartConnectedAnimation("image");
        }

        /// <summary>
        ///     Refresh when appearing
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            var sketch = await SketchManager.DefaultManager.GetSketchAsync(SketchId);

            BindingContext = sketch;

            if (Sketch != null)
            {
                var rating = await SketchManager.DefaultManager.GetRatingAsync(SketchId);

                UpdateLikeButton(rating);

                //await UpdateCommentsAsync();

                var position = new Position(Sketch.Latitude, Sketch.Longitude);

                var span = MapSpan.FromCenterAndRadius(position,
                    Distance.FromMiles(1.0));

                Map.MoveToRegion(span);

                var pushpin = new Pin
                {
                    Position = position,
                    Address = Sketch.Address,
                    Label = Sketch.Title
                };

                Map.Pins.Add(pushpin);
            }
        }

        private void UpdateLikeButton(IRating rating)
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
            var page = DependencyService.Get<IEditSketchPage>(DependencyFetchTarget.NewInstance);

            page.SketchId = SketchId;

            await Navigation.PushModalAsync(page as Page, true);

            //if (SketchManager.DefaultManager.CurrentClient.CurrentUser == null)
            //{
            //    var authenticated = await App.Authenticator.AuthenticateAsync();

            //    if (!authenticated)
            //        return;

            //    var currentUser = await SketchManager.DefaultManager.GetCurrentUserAsync();

            //    if (Sketch != null && Sketch.CreatedBy != currentUser.Id)
            //    {
            //        await DisplayAlert(
            //            Properties.Resources.EditSketch,
            //            Properties.Resources.OnlyEditOwnSketches,
            //            Properties.Resources.OK);
            //    }
            //}

            // EditSketch.IsVisible = true;
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

            var rating = await SketchManager.DefaultManager.GetRatingAsync(SketchId);

            if (rating == null)
                rating = new Rating
                {
                    IsHeart = true,
                    SketchId = SketchId
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

            if (person.Id != Sketch.CreatedBy && !person.IsAdministrator)
            {
                await DisplayAlert("Cannot Delete Sketch",
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "Only {0} or an administrator can delete this sketch.", Sketch.CreatedByName),
                    Properties.Resources.OK);

                return;
            }

            var response = await DisplayAlert(
                Properties.Resources.DeleteSketch,
                Properties.Resources.PressOKToDeleteSketch,
                Properties.Resources.OK,
                Properties.Resources.Cancel);

            if (!response) return;

            await SketchManager.DefaultManager.DeleteAsync(Sketch);

            await Navigation.PopAsync(true);
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
                Url = string.Format(
                    CultureInfo.InvariantCulture,
                    "http://urbansketchers.azurewebsites.net/sketch.html?id={0}",
                    Sketch.Id)
            };

            CrossShare.Current.Share(message);
        }

        private async void OnComment(object sender, EventArgs e)
        {
            var page = DependencyService.Get<ISketchCommentsPage>(DependencyFetchTarget.NewInstance);

            page.SketchId = SketchId;

            Image.Prepare("Image");

            await Navigation.PushModalAsync(page as Page, true);

            //_rating = await SketchManager.DefaultManager.GetRatingAsync(SketchId);

            //if (_rating == null)
            //    _rating = new Rating
            //    {
            //        SketchId = SketchId
            //    };

            //CommentEditor.Text = _rating.Comment;
            //ViolationSwitch.IsToggled = _rating.IsViolation;
            //CommentPanel.IsVisible = true;
        }


        //private async void OnInappropriate(object sender, EventArgs e)
        //{
        //    await DisplayAlert(Properties.Resources.InappropraiteSketch,
        //        Properties.Resources.InappropriateSketchDescription,
        //        Properties.Resources.OK);
        //}

        /// <summary>
        ///     Navigate to the picture page for the image when tapped
        /// </summary>
        /// <param name="sender">the image</param>
        /// <param name="e">the event arguments</param>
        private async void OnImageTapped(object sender, EventArgs e)
        {
            Image.Prepare("Image");

            await Navigation.PushModalAsync(new PicturePage
            {
                ImageSource = Image.Source
            });
        }
    }
}