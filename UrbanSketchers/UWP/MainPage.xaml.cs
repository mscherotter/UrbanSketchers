using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.UI.Xaml;
using Microsoft.WindowsAzure.MobileServices;
using UrbanSketchers;
using Xamarin;

namespace UWP
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : IAuthenticate
    {
        private const string UriScheme = "urbansketchersauth";

        private MobileServiceUser _user;

        private readonly PasswordVault _passwordVault;

        public MainPage()
        {
            _passwordVault = new PasswordVault();

            InitializeComponent();

            UrbanSketchers.App.Init(this);

            SketchManager.DefaultManager.ThumbnailGenerator = new ThumbnailGenerator();

            LoadApplication(new UrbanSketchers.App());

            var bingMapsKey = Application.Current.Resources["BingMapsKey"].ToString();

            FormsMaps.Init(bingMapsKey);

            InitializeMobileService();
        }

        private bool InitializeMobileService()
        {
            var resourceName = MobileServiceAuthenticationProvider.Facebook.ToString();

            try
            {
                var credentials = _passwordVault.FindAllByResource(resourceName);

                var credential = credentials.FirstOrDefault();

                if (credential != null)
                {
                    var user = new MobileServiceUser(credential.UserName);

                    credential.RetrievePassword();

                    user.MobileServiceAuthenticationToken = credential.Password;

                    _user = user;

                    SketchManager.DefaultManager.CurrentClient.CurrentUser = _user;

                    return true;
                }
            }
            catch (Exception)
            {
                // Exception is thrown if no credentials are found
            }

            return false;
        }

        /// <summary>
        /// Authenticate the mobile app with Facebook.
        /// </summary>
        /// <returns>an async task with a boolean value indicating whether the authentication was successful.</returns>
        public async Task<bool> AuthenticateAsync()
        {
            if (InitializeMobileService())
            {
                return true;
            }

            try
            {
                var resourceName = MobileServiceAuthenticationProvider.Facebook.ToString();

                _user = await SketchManager.DefaultManager.CurrentClient.LoginAsync(
                    MobileServiceAuthenticationProvider.Facebook, UriScheme);

                if (_user != null)
                {
                    var credential = new PasswordCredential(
                        resourceName,
                        _user.UserId, 
                        _user.MobileServiceAuthenticationToken);

                    _passwordVault.Add(credential);

                    return true;
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return false;
        }
    }
}