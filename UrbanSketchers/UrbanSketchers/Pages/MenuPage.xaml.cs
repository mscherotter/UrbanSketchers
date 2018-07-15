using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Autofac;
using UrbanSketchers.Commands;
using UrbanSketchers.Core;
using UrbanSketchers.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Pages
{
    /// <summary>
    ///     Menu page
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage
    {
        private object _lastItemSelected;
        private string _searchText;
        private string _signInImage;
        private string _signInText;

        /// <summary>
        ///     Initializes a new instance of the MenuPage class.
        /// </summary>
        public MenuPage()
        {
            InitializeComponent();

            PinToStartButton.Command = App.PinToStartCommand;

            string sketchMapIcon = null;
            string urbanSketchersIcon = null;
            string sketchesIcon = null;
            string aboutIcon = null;
            string myIcon = null;
            //string pinToStartIcon = null;
            //string signInIcon = null;

            switch (Device.RuntimePlatform)
            {
                case Device.UWP:
                    sketchMapIcon = "Assets/SketchMap.png";
                    urbanSketchersIcon = "Assets/UrbanSketchers.png";
                    sketchesIcon = "Assets/Sketches.png";
                    aboutIcon = "Assets/About.png";
                    myIcon = "Assets/SignedIn.png";
                    //pinToStartIcon = "Assets/PinToStart.png";
                    //signInIcon = "Assets/SignIn.png";
                    break;
            }

            //_signInItem = new NavigationMenuItem()
            //{
            //    Label = Properties.Resources.SignIn,
            //    Icon = signInIcon,
            //    Command = new RelayCommand<object>(SignIn)
            //};

            Items = new ObservableCollection<NavigationMenuItem>(new[]
            {
                new NavigationMenuItem
                {
                    Label = Properties.Resources.SketchMap,
                    Command = new NavigationCommand<IMapPage>(),
                    Icon = sketchMapIcon
                },
                new NavigationMenuItem
                {
                    Label = Properties.Resources.Sketches,
                    Command = new NavigationCommand<ISketchesPage>(),
                    Icon = sketchesIcon
                },
                new NavigationMenuItem
                {
                    Label = Properties.Resources.UrbanSketchers,
                    Command = new NavigationCommand<IPeoplePage>(),
                    Icon = urbanSketchersIcon
                },
                new NavigationMenuItem
                {
                    Label = Properties.Resources.MySketches,
                    Command = new NavigationCommand<IMySketchesPage>(),
                    Icon = myIcon
                }
            });

            //PinToStartButton.IsVisible = Device.RuntimePlatform == Device.UWP)
            //{

            //    Items.Add(new NavigationMenuItem
            //    {
            //        Label = "Pin to start",
            //        Icon = pinToStartIcon,
            //        Command = App.PinToStartCommand
            //    });
            //}

            Items.Add(new NavigationMenuItem
            {
                Label = Properties.Resources.AboutUrbanSketches,
                Icon = aboutIcon,
                Command = new NavigationCommand<IAboutPage>()
            });

            SignInCommand = new RelayCommand<object>(SignIn);

            BindingContext = this;
        }

        /// <summary>
        ///     Gets or sets the search text.
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    OnPropertyChanging();

                    _searchText = value;

                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        ///     Gets the sign-in/sign-out command
        /// </summary>
        public RelayCommand<object> SignInCommand { get; }

        /// <summary>
        ///     Gets the navigation menu items
        /// </summary>
        public ObservableCollection<NavigationMenuItem> Items { get; }

        /// <summary>
        ///     Gets or set sthe sign-in image
        /// </summary>
        public string SignInImage
        {
            get => _signInImage;

            private set
            {
                if (_signInImage != value)
                {
                    OnPropertyChanging();

                    _signInImage = value;

                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        ///     Gets or sets the sign-in text
        /// </summary>
        public string SignInText
        {
            get => _signInText;

            set
            {
                if (_signInText != value)
                {
                    OnPropertyChanging();

                    _signInText = value;

                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        ///     Update the user
        /// </summary>
        protected override async void OnAppearing()
        {
            App.Authenticator.SignedIn += Authenticator_SignedIn;

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
                var person = await Core.Container.Current.Resolve<ISketchManager>().GetCurrentUserAsync();

                if (person == null)
                {
                    if (Core.Container.Current.Resolve<ISketchManager>().CurrentClient.CurrentUser == null)
                    {
                        SignInImage = "Assets/SignIn.24.png";
                        SignInText = Properties.Resources.SignIn;
                    }
                    else
                    {
                        SignInImage = "Assets/SignedIn.24.png";
                        SignInText = Properties.Resources.SignOut;
                    }
                }
                else
                {
                    SignInImage = "Assets/SignedIn.24.png";
                    SignInText = person.Name;
                    //UserButton.Image = new UriImageSource
                    //{
                    //    Uri = new Uri(person.ImageUrl)
                    //};

                    //UserButton.Text = person.Name;
                }
            }
            catch (Exception)
            {
                if (Core.Container.Current.Resolve<ISketchManager>().CurrentClient.CurrentUser == null)
                {
                    SignInImage = "Assets/SignIn.24.png";
                    SignInText = Properties.Resources.SignIn;
                }
                else
                {
                    SignInImage = "Assets/SignedIn.24.png";
                    SignInText = Properties.Resources.SignOut;
                }
            }
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == _lastItemSelected)
                return;

            _lastItemSelected = e.SelectedItem;

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
            if (Core.Container.Current.Resolve<ISketchManager>().CurrentClient.CurrentUser == null)
            {
                await App.Authenticator.AuthenticateAsync();
            }
            else
            {
                if (!await DisplayAlert(
                    Properties.Resources.SignOut,
                    Properties.Resources.SignOutQuestion,
                    Properties.Resources.Yes,
                    Properties.Resources.No)) return;

                await App.Authenticator.LogoutAsync();

                await UpdateUserAsync();
            }
        }

        private async void SearchEntryCompleted(object sender, EventArgs e)
        {
            await NavigateToSearchResultsAsync();
        }

        private async Task NavigateToSearchResultsAsync()
        {
            var page = Container.Current.Resolve<ISketchesPage>();

            page.SearchText = SearchText;

            await App.NavigationPage.PushAsync(page as Page);

            if (Application.Current.MainPage is MasterDetailPage masterDetailPage)
                masterDetailPage.IsPresented = false;
        }

        private async void OnSearch(object sender, EventArgs e)
        {
            await NavigateToSearchResultsAsync();
        }
    }
}