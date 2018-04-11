using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using UrbanSketchers.Commands;
using UrbanSketchers.Data;
using Xamarin.Forms;

namespace UrbanSketchers.ViewModels
{
    /// <summary>
    /// Sketch comments page view model
    /// </summary>
    public class SketchCommentsPageViewModel : BaseDataObject, ISketchCommentsPageViewModel
    {
        private string _sketchId;
        private ISketch _sketch;
        private bool _isBusy;
        private bool _isEditing;
        private IRating _rating;
        private Page _page;

        /// <summary>
        /// Initializes a new instance of the RefreshCommand class.
        /// </summary>
        public SketchCommentsPageViewModel()
        {
            RefreshCommand = new RelayCommand<object>(OnRefresh, CanRefresh);
            EditCommand = new RelayCommand<object>(OnEdit, CanEdit);
            AcceptCommand = new RelayCommand<object>(OnAccept, CanAccept);
            CancelCommand = new RelayCommand<object>(OnCancel);
            InappropriateCommand = new RelayCommand<object>(OnInappropriate);
        }

        private async void OnInappropriate(object obj)
        {
            await Page.DisplayAlert(Properties.Resources.InappropraiteSketch,
                Properties.Resources.InappropriateSketchDescription,
                Properties.Resources.OK);
        }

        private void OnCancel(object obj)
        {
            IsEditing = false;
        }

        /// <summary>
        /// Gets or sets the rating
        /// </summary>
        public IRating Rating
        {
            get => _rating;
            set => SetProperty(ref _rating, value);
        }

        private bool CanAccept(object arg)
        {
            return !_isBusy && Page != null;
        }

        private async void OnAccept(object sender)
        {
            SetBusy(true);

            if (Rating.IsViolation && string.IsNullOrWhiteSpace(Rating.Comment))
            {
                await Page.DisplayAlert(
                    Properties.Resources.EnterComment,
                    string.Format(CultureInfo.CurrentCulture, Properties.Resources.AddCommentMessage,
                        Properties.Resources.InappropriateSketchDescription),
                    Properties.Resources.OK);
            }
            else
            {
                Rating.Comment = Rating.Comment.Truncate(256);

                await SketchManager.DefaultManager.SaveAsync(Rating);

                //CommentPanel.IsVisible = false;

                await LoadCommentsAsync();

                IsEditing = false;
            }

            SetBusy(false);
        }

        private void SetBusy(bool isBusy)
        {
            _isBusy = isBusy;

            AcceptCommand.RaiseCanExecuteChanged();
            RefreshCommand.RaiseCanExecuteChanged();
            EditCommand.RaiseCanExecuteChanged();
        }

        private bool CanEdit(object arg)
        {
            return !IsEditing && !_isBusy;
        }

        private async void OnEdit(object obj)
        {
            IsEditing = true;

            if (Rating == null)
            {
                Rating = await SketchManager.DefaultManager.GetRatingAsync(SketchId);
            }

            if (Rating == null)
            { 
                Rating = Core.Container.Current.Resolve<IRating>();

                Rating.SketchId = SketchId;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user is editing their comment
        /// </summary>
        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        private bool CanRefresh(object arg)
        {
            return !_isBusy;
        }

        private async void OnRefresh(object obj)
        {
            SetBusy(true);

            await LoadCommentsAsync();

            SetBusy(false);
        }

        /// <summary>
        /// Gets the comments for a sketch
        /// </summary>
        public ObservableCollection<IRating> Comments { get; } = new ObservableCollection<IRating>();

        /// <summary>
        /// Gets or sets the sketch Id
        /// </summary>
        public string SketchId
        {
            get => _sketchId;
            set
            {
               if (_sketchId != value)
               {
                   _sketchId = value;

                   LoadSketch();
               }
            }
        }

        /// <summary>
        /// Gets or sets the page
        /// </summary>
        public Page Page
        {
            get => _page;
            set
            {
                if (_page != value)
                {
                    _page = value;

                    AcceptCommand.RaiseCanExecuteChanged();
                }
            } }

        /// <summary>
        /// Gets the sketch
        /// </summary>
        public ISketch Sketch
        {
            get => _sketch;
            private set => SetProperty(ref _sketch, value);
        }

        /// <summary>
        /// Gets the Refresh command
        /// </summary>
        public RelayCommand<object> RefreshCommand { get; }

        /// <summary>
        /// Gets the edit command
        /// </summary>
        public RelayCommand<object> EditCommand { get; }

        /// <summary>
        /// Gets or sets the accept command
        /// </summary>
        public RelayCommand<object> AcceptCommand { get; }

        /// <summary>
        /// Gets or sets the cancel command
        /// </summary>
        public RelayCommand<object> CancelCommand { get; }

        /// <summary>
        /// Gets or sets the inappropriate command
        /// </summary>
        public RelayCommand<object> InappropriateCommand { get; }

        private async void LoadSketch()
        {
            Sketch = await SketchManager.DefaultManager.GetSketchAsync(SketchId);

            if (Sketch == null)
            {
                return;
            }

            await LoadCommentsAsync();
        }

        private async Task LoadCommentsAsync()
        {
            var ratings = from item in await SketchManager.DefaultManager.GetRatingsAsync(Sketch.Id)
                where !string.IsNullOrWhiteSpace(item.Comment)
                select item;

            Comments.SetRange(ratings);
        }
    }
}
