using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using UrbanSketchers.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SketchesPage : ContentPage
    {
        private SketchManager _sketchManager;

        public ObservableCollection<Sketch> Items { get; set; }

        public SketchesPage()
        {
            InitializeComponent();

            Items = new ObservableCollection<Sketch>
            {
                new Sketch
                {
                    Title = "Sagrada Familia"
                },
                new Sketch
                {
                    Title = "Barcelona Pavillion"
                },
                new Sketch
                {
                    Title = "Barcelona Cathedral"
                }
            };

            BindingContext = this;

            _sketchManager = SketchManager.DefaultManager;

            // OnPlatform<T> doesn't currently support the "Windows" target platform, so we have this check here.
            if (_sketchManager.IsOfflineEnabled && Device.OS == TargetPlatform.Windows)
            {
                var syncButton = new Button
                {
                    Text = "Sync items",
                    HeightRequest = 30
                };
                ////syncButton.Clicked += OnSyncItems;

                //buttonsPanel.Children.Add(syncButton);
            }

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await RefreshItems(true, syncItems: true);
        }

        private async Task RefreshItems(bool showActivityIndicator, bool syncItems)
        {
            //using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
            {
                var sketches = await _sketchManager.GetSketchsAsync();

                Items.Clear();

                foreach (var item in sketches)
                {
                    Items.Add(item);
                }
                //todoList.ItemsSource = await manager.GetTodoItemsAsync(syncItems);
            }
        }

        async void Handle_ItemTapped(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        private class ActivityIndicatorScope : IDisposable
        {
            private bool showIndicator;
            private ActivityIndicator indicator;
            private Task indicatorDelay;

            public ActivityIndicatorScope(ActivityIndicator indicator, bool showIndicator)
            {
                this.indicator = indicator;
                this.showIndicator = showIndicator;

                if (showIndicator)
                {
                    indicatorDelay = Task.Delay(2000);
                    SetIndicatorActivity(true);
                }
                else
                {
                    indicatorDelay = Task.FromResult(0);
                }
            }

            private void SetIndicatorActivity(bool isActive)
            {
                this.indicator.IsVisible = isActive;
                this.indicator.IsRunning = isActive;
            }

            public void Dispose()
            {
                if (showIndicator)
                {
                    indicatorDelay.ContinueWith(t => SetIndicatorActivity(false), TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }
    }
}