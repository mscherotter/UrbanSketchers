using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UrbanSketchers.Commands;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Views
{
    /// <summary>
    ///     Menu page
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage
    {
        /// <summary>
        ///     Initializes a new instance of the MenuPage class.
        /// </summary>
        public MenuPage()
        {
            InitializeComponent();

            string sketchMapIcon = null;
            string urbanSketchersIcon = null;
            string sketchesIcon = null;
            string aboutIcon = null;
            string pinToStartIcon = null;

            switch (Device.RuntimePlatform)
            {
                case Device.UWP:
                    sketchMapIcon = "Assets/SketchMap.png";
                    urbanSketchersIcon = "Assets/UrbanSketchers.png";
                    sketchesIcon = "Assets/Sketches.png";
                    aboutIcon = "Assets/About.png";
                    pinToStartIcon = "Assets/PinToStart.png";
                    break;
            }

            Items = new ObservableCollection<NavigationMenuItem>(new[]
            {
                new NavigationMenuItem
                {
                    Label = Properties.Resources.SketchMap,
                    Command = new NavigationCommand<MapPage>(),
                    Icon = sketchMapIcon
                },
                new NavigationMenuItem
                {
                    Label = Properties.Resources.Sketches,
                    Command = new NavigationCommand<SketchesPage>(),
                    Icon = sketchesIcon
                },
                new NavigationMenuItem
                {
                    Label = Properties.Resources.UrbanSketchers,
                    Command = new NavigationCommand<PeoplePage>(),
                    Icon = urbanSketchersIcon
                }
            });

            if (Device.RuntimePlatform == Device.UWP)
            {
                Items.Add(new NavigationMenuItem
                {
                    Label = "Pin to start",
                    Icon = pinToStartIcon,
                    Command = App.PinToStartCommand
                });
            }

            Items.Add(new NavigationMenuItem
                {
                    Label = Properties.Resources.AboutUrbanSketches,
                    Icon = aboutIcon,
                    Command = new NavigationCommand<AboutPage>()
                }
            );

            BindingContext = this;

            App.Authenticator.SignedIn += Authenticator_SignedIn;
        }

        /// <summary>
        ///     Gets the navigation menu items
        /// </summary>
        public ObservableCollection<NavigationMenuItem> Items { get; }

        /// <summary>
        ///     Update the user
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await UpdateUserAsync();
        }

        private async void Authenticator_SignedIn(object sender, EventArgs e)
        {
            await UpdateUserAsync();
        }

        private async Task UpdateUserAsync()
        {
            try
            {
                var person = await SketchManager.DefaultManager.GetCurrentUserAsync();

                if (person == null)
                {
                    UserImage.Source = new FileImageSource
                    {
                        File = "Assets/SignedIn.png"
                    };

                    UserName.Text = "Sign out";
                }
                else
                {
                    UserImage.Source = new UriImageSource
                    {
                        Uri = new Uri(person.ImageUrl)
                    };

                    UserName.Text = person.Name;
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is NavigationMenuItem item)
            {
                if (item.Command.CanExecute(null))
                    item.Command.Execute(null);

                if (sender is ListView listView)
                    listView.SelectedItem = null;
            }
        }

        private void SignIn(object sender, EventArgs e)
        {
            App.Authenticator.Authenticate();
        }
    }
}