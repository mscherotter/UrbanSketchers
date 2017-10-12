using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web.Core;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI.ApplicationSettings;
using Microsoft.WindowsAzure.MobileServices;
using UrbanSketchers;

namespace UWP
{
    /// <summary>
    ///     UWP Mobile Service initialization
    /// </summary>
    internal class MobileServiceInit : IAuthenticate
    {
        private const string UriScheme = "urbansketchesauth";
        private readonly PasswordVault _passwordVault;
        private MobileServiceUser _user;

        public MobileServiceInit()
        {
            _passwordVault = new PasswordVault();

            Initialize();

            AccountsSettingsPane.GetForCurrentView().AccountCommandsRequested += MainPage_AccountCommandsRequested;
        }

        public event EventHandler SignedIn;

        /// <summary>
        ///     Authenticate the mobile app with Facebook.
        /// </summary>
        /// <returns>an async task with a boolean value indicating whether the authentication was successful.</returns>
        public bool Authenticate()
        {
            if (Initialize())
                return true;

            AccountsSettingsPane.Show();

            return false;
        }

        private async void MainPage_AccountCommandsRequested(AccountsSettingsPane sender,
            AccountsSettingsPaneCommandsRequestedEventArgs args)
        {
            var deferral = args.GetDeferral();

            var msaProvider =
                await WebAuthenticationCoreManager.FindAccountProviderAsync("https://login.microsoft.com", "consumers");

            var command = new WebAccountProviderCommand(msaProvider, GetMsaTokenAsync);

            args.WebAccountProviderCommands.Add(command);

            var facebookProvider =
                await WebAuthenticationCoreManager.FindAccountProviderAsync("https://www.facebook.com");

            args.WebAccountProviderCommands.Add(new WebAccountProviderCommand(facebookProvider, FacebookLogin));

            deferral.Complete();
        }

        internal bool Initialize()
        {
            if (!ApplicationData.Current.LocalSettings.Values.TryGetValue("MobileServiceAuthenticationProvider",
                out object value)) return false;

            var resourceName = value.ToString();

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

                    SignedIn?.Invoke(this, new EventArgs());

                    return true;
                }
            }
            catch (Exception)
            {
                // Exception is thrown if no credentials are found
            }

            return false;
        }

        private async void FacebookLogin(WebAccountProviderCommand command)
        {
            await LoginAsync(MobileServiceAuthenticationProvider.Facebook);
        }

        private async void GetMsaTokenAsync(WebAccountProviderCommand command)
        {
            await LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount);
        }

        private async Task LoginAsync(MobileServiceAuthenticationProvider provider)
        {
            try
            {
                var resourceName = provider.ToString();

                _user = await SketchManager.DefaultManager.CurrentClient.LoginAsync(
                    provider, UriScheme);

                if (_user != null)
                {
                    var credential = new PasswordCredential(
                        resourceName,
                        _user.UserId,
                        _user.MobileServiceAuthenticationToken);

                    _passwordVault.Add(credential);

                    ApplicationData.Current.LocalSettings.Values["MobileServiceAuthenticationProvider"] = resourceName;

                    SignedIn?.Invoke(this, new EventArgs());
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}