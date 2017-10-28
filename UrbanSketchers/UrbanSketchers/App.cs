using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using UrbanSketchers.Views;
using Xamarin.Forms;

namespace UrbanSketchers
{
    /// <summary>
    /// Authenticate interface
    /// </summary>
    public interface IAuthenticate
    {
        /// <summary>
        /// Authenticate Azure Mobile App
        /// </summary>
        /// <returns>true if successful</returns>
        bool Authenticate();

        /// <summary>
        /// Signed in event
        /// </summary>
        event EventHandler SignedIn;

        Task LogoutAsync();
    }

    public interface IThumbnailGenerator
    {
        Task<Stream> CreateThumbnailAsync(byte[] data);
    }

    /// <summary>
    /// Urban sketchers app
    /// </summary>
    public class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the App class
        /// </summary>
        public App()
        {
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS || Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
            {
                // determine the correct, supported .NET culture
                var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
                UrbanSketchers.Properties.Resources.Culture = ci; // set the RESX for resource localization
                DependencyService.Get<ILocalize>().SetLocale(ci); // set the Thread for locale-aware methods
            }

            var menuPage = new MenuPage();

            NavigationPage = new NavigationPage(new HomePage());

            var rootPage = new RootPage();

            rootPage.Master = menuPage;
            rootPage.Detail = NavigationPage;
            //{
            //    Master = menuPage,
            //    Detail = NavigationPage
            //};

            MainPage = rootPage;

            // The root page of your application
            //MainPage = new TabbedPage
            //{
            //    Children =
            //    {
            //        new NavigationPage(new MapPage())
            //        {
            //            Title = "Sketch Map"
            //            //Icon = Xamarin.Forms.Device.OnPlatform("tab_feed.png", null, null)
            //        },
            //        new NavigationPage(new SketchesPage())
            //        {
            //            Title = "Sketches"
            //            //Icon = Xamarin.Forms.Device.OnPlatform("tab_about.png", null, null)
            //        },
            //        new NavigationPage(new PeoplePage())
            //        {
            //            Title = "Urban Sketchers"
            //            //Icon = Xamarin.Forms.Device.OnPlatform("tab_about.png", null, null)
            //        }
            //    }
            //};
        }

        public static NavigationPage NavigationPage { get; private set; }

        public static IAuthenticate Authenticator { get; private set; }

        /// <summary>
        /// Gets or sets the pin to start command
        /// </summary>
        public static ICommand PinToStartCommand { get; set; }

        public static void Init(IAuthenticate authenticator)
        {
            Authenticator = authenticator;
        }

        /// <summary>
        ///     Start mobile center analytics
        /// </summary>
        protected override void OnStart()
        {
            //MobileCenter.Start("ios=132544fa-8be4-4fbc-a1f0-ba85d44880a2;" +
            //                   "uwp={132544fa-8be4-4fbc-a1f0-ba85d44880a2};" +
            //                   "android={Your Android App secret here}",
            //    typeof(Analytics), typeof(Crashes));

            MobileCenter.Start("uwp={132544fa-8be4-4fbc-a1f0-ba85d44880a2};",
                typeof(Analytics), typeof(Crashes));
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}