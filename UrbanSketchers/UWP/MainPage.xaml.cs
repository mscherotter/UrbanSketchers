using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using UrbanSketchers;
using Xamarin;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : IAuthenticate
    {
        private const string BingMapsKey =
                "B6p5hudPVN61Ykpp6D7W~JW-lf-G0P7wmsDcDrMWFuw~AsZdr0PHfOeWe9qmPtHDbuONPySTrgN47oWYdvD84J67bvxcMbXDQEnZCz6XWwR1"
            ;

        private MobileServiceUser _user;

        public MainPage()
        {
            InitializeComponent();

            UrbanSketchers.App.Init(this);

            LoadApplication(new UrbanSketchers.App());

            FormsMaps.Init(BingMapsKey);
        }

        public async Task<bool> AuthenticateAsync()
        {
            try
            {
                _user = await SketchManager.DefaultManager.CurrentClient.LoginAsync(
                    MobileServiceAuthenticationProvider.Facebook, "urbansketchers");

                if (_user != null)
                    return true;
            }
            catch (Exception)
            {
                // ignored
            }

            return false;
        }
    }
}