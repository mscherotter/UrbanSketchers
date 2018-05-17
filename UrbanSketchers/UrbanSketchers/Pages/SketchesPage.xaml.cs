using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.WindowsAzure.MobileServices;
using UrbanSketchers.Data;
using UrbanSketchers.Interfaces;
using UrbanSketchers.Support;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Pages
{
    /// <summary>
    ///     Sketches page
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SketchesPage : ISketchesPage
    {
        private readonly ISketchManager _sketchManager;

        /// <summary>
        ///     Initializes a new instance of the SketchesPage class.
        /// </summary>
        public SketchesPage(ISketchManager sketchManager)
        {
            InitializeComponent();

            BindingContext = this;

            _sketchManager = sketchManager;
        }

        /// <summary>
        /// Gets the sketch  manager
        /// </summary>
        public ISketchManager SketchManager => _sketchManager; 

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
            using (new ActivityIndicatorScope(ActivityIndicator, true))
            {
                //using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
                {
                    if (!string.IsNullOrWhiteSpace(SearchText))
                    {
                        var results = await _sketchManager.SearchAsync(SearchText);

                        Title = string.Format(
                            CultureInfo.CurrentCulture,
                            Properties.Resources.XResultsForY,
                            0,
                            SearchText);

                        SketchList.ItemsSource = results;

                        Title = string.Format(CultureInfo.CurrentCulture, "{0} Sketches", results.Count());
                    }
                    else if (!string.IsNullOrWhiteSpace(PersonId))
                    {
                        var sketches = await _sketchManager.GetSketchsAsync(PersonId);

                        if (sketches.Any())
                        {
                            if (sketches is ITotalCountProvider collection)
                            {
                                Title = string.Format(
                                    CultureInfo.CurrentCulture,
                                    Properties.Resources.SketchesByFormat,
                                    collection.TotalCount,
                                    sketches.First().CreatedByName);
                            }
                        }
                        SketchList.ItemsSource = sketches;
                    }
                    else
                    {
                        var sketches = await _sketchManager.GetSketchsAsync();

                        if (sketches is ITotalCountProvider collection)
                        {
                            Title = string.Format(
                                CultureInfo.CurrentCulture,
                                Properties.Resources.NSketches,
                                collection.TotalCount);
                        }

                        SketchList.ItemsSource = sketches;
                    }
                }
            }
        }

        private async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is ISketch sketch)
            {
                var page = Core.Container.Current.Resolve<ISketchPage>();

                page.SketchId = sketch.Id;

                await Navigation.PushAsync(page as Page, true);
            }
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