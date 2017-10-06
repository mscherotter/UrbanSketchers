using System.Threading.Tasks;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using UrbanSketchers.Views;
using Xamarin.Forms;

namespace UrbanSketchers
{
    public interface IAuthenticate
    {
        Task<bool> AuthenticateAsync();
    }

    public class App : Application
    {
        public App()
        {
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