﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web.Core;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Microsoft.WindowsAzure.MobileServices;
using UrbanSketchers;
using Autofac;
using UrbanSketchers.Interfaces;

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
        private readonly ISketchManager _sketchManager;
        public MobileServiceInit(ISketchManager sketchManager)
        {
            _sketchManager = sketchManager;
            _passwordVault = new PasswordVault();

            Initialize();

            //AccountsSettingsPane.GetForCurrentView().AccountCommandsRequested += MainPage_AccountCommandsRequested;
        }

        public event EventHandler SignedIn;

        /// <summary>
        ///     Authenticate the mobile app with Facebook.
        /// </summary>
        /// <returns>an async task with a boolean value indicating whether the authentication was successful.</returns>
        public async Task<bool> AuthenticateAsync()
        {
            string message = string.Empty;
            var success = false;
            try
            {
                if (_user == null)
                {
                    await LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount);

                    if (_user != null)
                    {
                        success = true;
                        message = string.Format("You are now signed in as {0}.", _user.UserId);
                    }
                }
            }
            catch (Exception e)
            {
                message = string.Format("Authentication failed: {0}", e.Message);
            }

            await new MessageDialog(message, "Sign-in results").ShowAsync();

            return success;
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

            PasswordCredential credential = null;

            try
            {
                var credentials = _passwordVault.FindAllByResource(resourceName);

                credential = credentials.FirstOrDefault();

                if (credential != null)
                {
                    var user = new MobileServiceUser(credential.UserName);

                    credential.RetrievePassword();

                    user.MobileServiceAuthenticationToken = credential.Password;

                    _user = user;

                    _sketchManager.CurrentClient.CurrentUser = _user;

                    //_user = await SketchManager.DefaultManager.CurrentClient.RefreshUserAsync();

                    SignedIn?.Invoke(this, new EventArgs());

                    return true;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing mobile service with stored credential: {e.Message}");

                _sketchManager.CurrentClient.CurrentUser = null;

                //if (credential != null)
                //{
                //    _passwordVault.Remove(credential);
                //}
                _user = null;
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

                _user = await _sketchManager.CurrentClient.LoginAsync(
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

        public async Task LogoutAsync()
        {
            if (_sketchManager.CurrentClient.CurrentUser == null)
            {
                return;
            }

            await _sketchManager.CurrentClient.LogoutAsync();

            if (ApplicationData.Current.LocalSettings.Values.TryGetValue("MobileServiceAuthenticationProvider",
                out object value))
            {
                try
                {
                    var resourceName = value.ToString();

                    var credentials = _passwordVault.FindAllByResource(resourceName);

                    var credential = credentials.FirstOrDefault();

                    if (credential != null)
                    {
                        _passwordVault.Remove(credential);
                    }
                    var keyValue = (from item in ApplicationData.Current.LocalSettings.Values
                        where item.Key == "MobileServiceAuthenticationProvider"
                        select item).FirstOrDefault();

                    ApplicationData.Current.LocalSettings.Values.Remove(keyValue);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
    }
}