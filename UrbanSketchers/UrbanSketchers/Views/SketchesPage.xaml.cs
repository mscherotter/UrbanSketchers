using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UrbanSketchers.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Views
{
    /// <summary>
    ///     Sketches page
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SketchesPage
    {
        private readonly SketchManager _sketchManager;

        /// <summary>
        ///     Initializes a new instance of the SketchesPage class.
        /// </summary>
        public SketchesPage()
        {
            InitializeComponent();

            //Items = new ObservableCollection<Sketch>();

            BindingContext = this;

            _sketchManager = SketchManager.DefaultManager;
        }

        // public ObservableCollection<Sketch> Items { get; set; }

        /// <summary>
        ///     Gets the person Id
        /// </summary>
        public string PersonId { get; internal set; }

        /// <summary>
        /// Gets or sets the search text
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        ///     Refresh the items when appearing
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await RefreshItemsAsync();
        }

        private async Task RefreshItemsAsync()
        {
            //using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
            {
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    var results = await SketchManager.DefaultManager.SearchAsync(SearchText);

                    Title = string.Format(
                        CultureInfo.CurrentCulture,
                        Properties.Resources.XResultsForY,
                        results.TotalCount,
                        SearchText);

                    SketchList.ItemsSource = results;
                }
                else if (!string.IsNullOrWhiteSpace(PersonId))
                {
                    var sketches = await _sketchManager.GetSketchsAsync(PersonId);

                    if (sketches.Any())
                        Title = string.Format(
                            CultureInfo.CurrentCulture,
                            Properties.Resources.SketchesByFormat,
                            sketches.Count,
                            sketches.First().CreatedByName);

                    SketchList.ItemsSource = sketches;
                }
                else
                {
                    var sketches = await _sketchManager.GetSketchsAsync();

                    Title = string.Format(
                        CultureInfo.CurrentCulture,
                        Properties.Resources.NSketches,
                        sketches?.TotalCount ?? 0);

                    SketchList.ItemsSource = sketches;
                }
            }
        }

        private async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Sketch sketch)
                await Navigation.PushAsync(new SketchPage
                {
                    SketchId = sketch.Id
                });
        }

        private async void OnRefresh(object sender, EventArgs e)
        {
            await RefreshItemsAsync();
        }

        //private class ActivityIndicatorScope : IDisposable
        //{
        //    private readonly ActivityIndicator indicator;
        //    private readonly Task indicatorDelay;
        //    private readonly bool showIndicator;

        //    public ActivityIndicatorScope(ActivityIndicator indicator, bool showIndicator)
        //    {
        //        this.indicator = indicator;
        //        this.showIndicator = showIndicator;

        //        if (showIndicator)
        //        {
        //            indicatorDelay = Task.Delay(2000);
        //            SetIndicatorActivity(true);
        //        }
        //        else
        //        {
        //            indicatorDelay = Task.FromResult(0);
        //        }
        //    }

        //    public void Dispose()
        //    {
        //        if (showIndicator)
        //            indicatorDelay.ContinueWith(t => SetIndicatorActivity(false),
        //                TaskScheduler.FromCurrentSynchronizationContext());
        //    }

        //    private void SetIndicatorActivity(bool isActive)
        //    {
        //        indicator.IsVisible = isActive;
        //        indicator.IsRunning = isActive;
        //    }
        //}
    }
}