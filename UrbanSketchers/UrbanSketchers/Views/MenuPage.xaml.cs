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
            string signInIcon = null;

            switch (Device.RuntimePlatform)
            {
                case Device.UWP:
                    sketchMapIcon = "Assets/SketchMap.png";
                    urbanSketchersIcon = "Assets/UrbanSketchers.png";
                    sketchesIcon = "Assets/Sketches.png";
                    aboutIcon = "Assets/About.png";
                    pinToStartIcon = "Assets/PinToStart.png";
                    signInIcon = "Assets/SignIn.png";
                    break;
            }

            _signInItem = new NavigationMenuItem()
            {
                Label = Properties.Resources.SignIn,
                Icon = signInIcon,
                Command = new RelayCommand<object>(SignIn)
            };

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

            Items.AddRange(new [] 
            {
                new NavigationMenuItem
                {
                    Label = Properties.Resources.AboutUrbanSketches,
                    Icon = aboutIcon,
                    Command = new NavigationCommand<AboutPage>()
                },
                _signInItem
            });

            BindingContext = this;

            App.Authenticator.SignedIn += Authenticator_SignedIn;
        }

        private NavigationMenuItem _signInItem;

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
                    if (SketchManager.DefaultManager.CurrentClient.CurrentUser == null)
                    {
                        _signInItem.Icon = "Assets/SignIn.png";
                        _signInItem.Label = "Sign in";
                    }
                    else
                    {
                        _signInItem.Icon = "Assets/SignedIn.png";
                        _signInItem.Label = "Sign out";
                    }
                }
                else
                {
                    _signInItem.Icon = "Assets/SignedIn.png";
                    _signInItem.Label = person.Name;
                    //UserButton.Image = new UriImageSource
                    //{
                    //    Uri = new Uri(person.ImageUrl)
                    //};

                    //UserButton.Text = person.Name;
                }
            }
            catch (Exception)
            {
                if (SketchManager.DefaultManager.CurrentClient.CurrentUser == null)
                {
                    _signInItem.Icon = "Assets/SignIn.png";
                    _signInItem.Label = "Sign in";
                }
                else
                {
                    _signInItem.Icon = "Assets/SignedIn.png";
                    _signInItem.Label = "Sign out";
                }
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

        private async void SignIn(object parameter)
        {
            if (SketchManager.DefaultManager.CurrentClient.CurrentUser == null)
            {
                App.Authenticator.Authenticate();
            }
            else
            {
                await App.Authenticator.LogoutAsync();

                await UpdateUserAsync();
            }
        }
    }
}