using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Microsoft.WindowsAzure.MobileServices;
using UrbanSketchers;

namespace UWP
{
    internal class MobileServiceInit : IAuthenticate
    {
        private readonly PasswordVault _passwordVault;
        private const string UriScheme = "urbansketchersauth";
        private MobileServiceUser _user;

        public MobileServiceInit()
        {
            _passwordVault = new PasswordVault();

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
