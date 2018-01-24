using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Push;
using UrbanSketchers.Controls;
using UrbanSketchers.Data;
using UrbanSketchers.Pages;
using UrbanSketchers.ViewModels;
using Xamarin.Forms;
using Device = Xamarin.Forms.Device;

namespace UrbanSketchers
{
    /// <summary>
    ///     Authenticate interface
    /// </summary>
    public interface IAuthenticate
    {
        /// <summary>
        ///     Authenticate Azure Mobile App
        /// </summary>
        /// <returns>true if successful</returns>
        Task<bool> AuthenticateAsync();

        /// <summary>
        ///     Signed in event
        /// </summary>
        event EventHandler SignedIn;

        /// <summary>
        ///     logout of Azure Mobile Service
        /// </summary>
        /// <returns>an async task</returns>
        Task LogoutAsync();
    }

    /// <summary>
    ///     Thumbnail generator interface
    /// </summary>
    public interface IThumbnailGenerator
    {
        /// <summary>
        ///     Creates a thumbnail
        /// </summary>
        /// <param name="data">the image data</param>
        /// <returns>an async task with the image stream of the thumbnail</returns>
        Task<Stream> CreateThumbnailAsync(byte[] data);
    }

    /// <summary>
    ///     Urban sketchers app
    /// </summary>
    public class App : Application
    {
        /// <summary>
        ///     Initializes a new instance of the App class
        /// </summary>
        public App()
        {
            DependencyService.Register<IAboutPage, AboutPage>();
            DependencyService.Register<IDrawingPage, DrawingPage>();
            DependencyService.Register<IEditSketchPage, EditSketchPage>();
            DependencyService.Register<IMapPage, MapPage>();
            DependencyService.Register<IPeoplePage, PeoplePage>();
            DependencyService.Register<IPerson, Person>();
            DependencyService.Register<IPersonPageViewModel, PersonPageViewModel>();
            DependencyService.Register<IRating, Rating>();
            DependencyService.Register<ISketch, Sketch>();
            DependencyService.Register<ISketchCommentsPage, SketchCommentsPage>();
            DependencyService.Register<ISketchCommentsPageViewModel, SketchCommentsPageViewModel>();
            DependencyService.Register<ISketchPage, SketchPage>();
            DependencyService.Register<ISketchPin, SketchPin>();
            DependencyService.Register<ISketchesPage, SketchesPage>();

            AppCenter.Start(
                "ios=132544fa-8be4-4fbc-a1f0-ba85d44880a2;"
                + "uwp=aefb0a99-2ded-4ae7-a6b9-23beb92efdae;"
                + "android=80ffbddd-540d-4a9b-98cb-94645dc3a880",
                typeof(Analytics),
                typeof(Crashes),
                typeof(Push));

            if (Device.RuntimePlatform == Device.iOS || Device.RuntimePlatform == Device.Android)
            {
                // determine the correct, supported .NET culture
                var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
                UrbanSketchers.Properties.Resources.Culture = ci; // set the RESX for resource localization
                DependencyService.Get<ILocalize>().SetLocale(ci); // set the Thread for locale-aware methods
            }

            //var menuPage = new MenuPage();

            //NavigationPage = new NavigationPage(new HomePage());

            var rootPage = new RootPage();

            NavigationPage = rootPage.Detail as NavigationPage;

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

        /// <summary>
        ///     Gets the navigation page
        /// </summary>
        public static NavigationPage NavigationPage { get; private set; }

        /// <summary>
        ///     Gets the authenticator
        /// </summary>
        public static IAuthenticate Authenticator { get; private set; }

        /// <summary>
        ///     Gets or sets the pin to start command
        /// </summary>
        public static ICommand PinToStartCommand { get; set; }

        /// <summary>
        ///     Initialize the authenticator
        /// </summary>
        /// <param name="authenticator">the authenticator</param>
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

            ////AppCenter.Start("132544fa-8be4-4fbc-a1f0-ba85d44880a2",
            ////    typeof(Analytics), typeof(Crashes));

            //MobileCenter.Start("uwp={132544fa-8be4-4fbc-a1f0-ba85d44880a2};",
            //    typeof(Analytics), typeof(Crashes));
        }

        /// <summary>
        ///     Application sleeping
        /// </summary>
        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        /// <summary>
        ///     Application resuming
        /// </summary>
        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}