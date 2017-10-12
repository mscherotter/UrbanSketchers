using System;
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
        private Rating _rating;

        /// <summary>
        /// Initializes a new instance of the SketchPage class.
        /// </summary>
        public SketchPage()
        {
            InitializeComponent();

            ShareItem.IsEnabled = CrossShare.IsSupported;

            Image.PropertyChanged += Image_PropertyChanged;
        }

        private void Image_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Renderer") return;

            if (!Image.IsLoading && Image.Width > 0 && Image.Height > 0 && Image.Control != null && Image.Source != null)
            {
                Image.StartConnectedAnimation("image");
            }
        }

        /// <summary>
        /// Gets the sketch Id
        /// </summary>
        public string SketchId { get; internal set; }

        /// <summary>
        /// Gets the <see cref="Sketch"/> as the binding context
        /// </summary>
        public Sketch Sketch => BindingContext as Sketch;

        /// <summary>
        /// Refresh when appearing
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

                await UpdateCommentsAsync();
            }
        }

        private async Task UpdateCommentsAsync()
        {
            var allRatings = await SketchManager.DefaultManager.GetRatingsAsync(Sketch.Id);

            Comments.ItemsSource = allRatings;
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
                var authenticated = App.Authenticator.Authenticate();

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
                if (App.Authenticator.Authenticate())
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
                Url = string.Format(CultureInfo.InvariantCulture, "http://urbansketchers.azurewebsites.net/sketch.html?id={0}", Sketch.Id)
            };

            CrossShare.Current.Share(message);
        }

        private async void OnComment(object sender, EventArgs e)
        {
            _rating = await SketchManager.DefaultManager.GetRatingAsync(SketchId);

            if (_rating == null)
                _rating = new Rating
                {
                    SketchId = SketchId
                };

            CommentEditor.Text = _rating.Comment;
            ViolationSwitch.IsToggled = _rating.IsViolation;
            CommentPanel.IsVisible = true;
        }

        private void OnCancelComment(object sender, EventArgs e)
        {
            CommentPanel.IsVisible = false;
        }

        /// <summary>
        /// Save the rating
        /// </summary>
        /// <param name="sender">the button</param>
        /// <param name="e">the event arguments</param>
        private async void OnAcceptComment(object sender, EventArgs e)
        {
            var button = sender as Button;

            if (button != null)
                button.IsEnabled = false;

            _rating.Comment = CommentEditor.Text;
            _rating.IsViolation = ViolationSwitch.IsToggled;

            if (_rating.IsViolation && string.IsNullOrWhiteSpace(_rating.Comment))
            {
                await DisplayAlert(
                    Properties.Resources.EnterComment,
                    string.Format(CultureInfo.CurrentCulture, Properties.Resources.AddCommentMessage, Properties.Resources.InappropriateSketchDescription),
                    Properties.Resources.OK);
            }
            else
            {
                await SketchManager.DefaultManager.SaveAsync(_rating);

                CommentPanel.IsVisible = false;

                await UpdateCommentsAsync();
            }

            if (button != null) button.IsEnabled = true;
        }

        private async void OnInappropriate(object sender, EventArgs e)
        {
            await DisplayAlert(Properties.Resources.InappropraiteSketch,
                Properties.Resources.InappropriateSketchDescription,
                Properties.Resources.OK);
        }
    }
}