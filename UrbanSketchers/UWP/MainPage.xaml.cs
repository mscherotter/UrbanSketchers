using System;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using UrbanSketchers;
using Xamarin;
using Autofac;
using UrbanSketchers.Interfaces;

namespace UWP
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private readonly SketchManager _sketchManager;

        //private const string UriScheme = "urbansketchesauth";

        //private MobileServiceUser _user;

        //private readonly PasswordVault _passwordVault;

        //public event EventHandler SignedIn;

        private readonly MobileServiceInit _mobileServiceInit;

        /// <summary>
        ///     Initializes a new instance of the MainPage class.
        /// </summary>
        public MainPage()
        {
            //_passwordVault = new PasswordVault();

            InitializeComponent();

            _sketchManager = new SketchManager();

            _mobileServiceInit = new MobileServiceInit(_sketchManager);

            UrbanSketchers.App.Init(_mobileServiceInit);

            UrbanSketchers.App.PinToStartCommand = new PinToStartCommand();

            LoadApplication(new UrbanSketchers.App(_sketchManager));

            UrbanSketchers.Core.Container.Current.Resolve<ISketchManager>().ThumbnailGenerator = new ThumbnailGenerator();


            var bingMapsKey = Application.Current.Resources["BingMapsKey"].ToString();

            FormsMaps.Init(bingMapsKey);
        }

        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    AccountsSettingsPane.GetForCurrentView().AccountCommandsRequested += MainPage_AccountCommandsRequested;
        //}

        //protected override void OnNavigatedFrom(NavigationEventArgs e)
        //{
        //    AccountsSettingsPane.GetForCurrentView().AccountCommandsRequested -= MainPage_AccountCommandsRequested;
        //}

        //private async void MainPage_AccountCommandsRequested(AccountsSettingsPane sender, AccountsSettingsPaneCommandsRequestedEventArgs args)
        //{
        //    var deferral = args.GetDeferral();

        //    var msaProvider =
        //        await WebAuthenticationCoreManager.FindAccountProviderAsync("https://login.microsoft.com", "consumers");

        //    var command = new WebAccountProviderCommand(msaProvider, GetMsaTokenAsync);

        //    args.WebAccountProviderCommands.Add(command);

        //    var facebookProvider =
        //        await WebAuthenticationCoreManager.FindAccountProviderAsync("https://www.facebook.com");

        //    args.WebAccountProviderCommands.Add (new WebAccountProviderCommand(facebookProvider, FacebookLogin));

        //    deferral.Complete();
        //}

        //private async void FacebookLogin(WebAccountProviderCommand command)
        //{
        //    await LoginAsync(MobileServiceAuthenticationProvider.Facebook);
        //}

        //private async void GetMsaTokenAsync(WebAccountProviderCommand command)
        //{
        //    await LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount);
        //}

        //private async Task LoginAsync(MobileServiceAuthenticationProvider provider)
        //{ 
        //    try
        //    {
        //        var resourceName = provider.ToString();

        //        _user = await SketchManager.DefaultManager.CurrentClient.LoginAsync(
        //            provider, UriScheme);

        //        if (_user != null)
        //        {
        //            var credential = new PasswordCredential(
        //                resourceName,
        //                _user.UserId,
        //                _user.MobileServiceAuthenticationToken);

        //            _passwordVault.Add(credential);

        //            ApplicationData.Current.LocalSettings.Values["MobileServiceAuthenticationProvider"] = resourceName;

        //            SignedIn?.Invoke(this, new EventArgs());
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        // ignored
        //    }
        //}

        //private bool InitializeMobileService()
        //{
        //    if (!ApplicationData.Current.LocalSettings.Values.TryGetValue("MobileServiceAuthenticationProvider",
        //        out object value)) return false;

        //    var resourceName = value.ToString();

        //    try
        //    {
        //        var credentials = _passwordVault.FindAllByResource(resourceName);

        //        var credential = credentials.FirstOrDefault();

        //        if (credential != null)
        //        {
        //            var user = new MobileServiceUser(credential.UserName);

        //            credential.RetrievePassword();

        //            user.MobileServiceAuthenticationToken = credential.Password;

        //            _user = user;

        //            SketchManager.DefaultManager.CurrentClient.CurrentUser = _user;

        //            return true;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        // Exception is thrown if no credentials are found
        //    }

        //    return false;
        //}

        /// <summary>
        ///     Authenticate the mobile app with Facebook.
        /// </summary>
        /// <returns>an async task with a boolean value indicating whether the authentication was successful.</returns>
        //public bool Authenticate()
        //{
        //    if (_mobileServiceInit.Initialize())
        //    {
        //        return true;
        //    }

        //    AccountsSettingsPane.Show();

        //    return false;
        //}
        private void UIElement_OnDrop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.Bitmap))
            {
            }
        }

        private async void OnDragOver(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.Bitmap))
            {
                e.AcceptedOperation = DataPackageOperation.Copy;

                return;
            }

            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var deferral = e.GetDeferral();

                var storageItems = await e.DataView.GetStorageItemsAsync();

                var imageFiles = from item in storageItems.OfType<StorageFile>()
                    where item.ContentType.StartsWith("image/")
                    select item;

                if (imageFiles.Any())
                    e.AcceptedOperation = DataPackageOperation.Copy;

                deferral.Complete();
            }
        }
    }
}