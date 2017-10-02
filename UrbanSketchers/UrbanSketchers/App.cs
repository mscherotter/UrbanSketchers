using System.Threading.Tasks;
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
            // The root page of your application
            MainPage = new TabbedPage
            {
                Children =
                {
                    new NavigationPage(new MapPage())
                    {
                        Title = "Sketch Map",
                        Icon = Device.OnPlatform("tab_feed.png", null, null)
                    },
                    new NavigationPage(new SketchesPage())
                    {
                        Title = "Sketches",
                        Icon = Device.OnPlatform("tab_about.png", null, null)
                    },
                    new NavigationPage(new PeoplePage())
                    {
                        Title = "Urban Sketchers",
                        Icon = Device.OnPlatform("tab_about.png", null, null)
                    }
                }
            };
        }

        public static IAuthenticate Authenticator { get; private set; }

        public static void Init(IAuthenticate authenticator)
        {
            Authenticator = authenticator;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
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