using Foundation;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.WindowsAzure.MobileServices;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace UrbanSketchers.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // Initialize Azure Mobile Apps
            CurrentPlatform.Init();

            // Initialize Xamarin Forms
            Forms.Init();

            LoadApplication(new App());

            ////AppCenter.Start("132544fa-8be4-4fbc-a1f0-ba85d44880a2",
            ////    typeof(Analytics), typeof(Crashes));

            return base.FinishedLaunching(app, options);
        }
    }
}