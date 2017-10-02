using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Microsoft.WindowsAzure.MobileServices;
using UrbanSketchers;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : IAuthenticate
    {
        private const string BingMapsKey =
                "B6p5hudPVN61Ykpp6D7W~JW-lf-G0P7wmsDcDrMWFuw~AsZdr0PHfOeWe9qmPtHDbuONPySTrgN47oWYdvD84J67bvxcMbXDQEnZCz6XWwR1";
        private MobileServiceUser _user;

        public MainPage()
        {
            this.InitializeComponent();

            UrbanSketchers.App.Init(this);

            LoadApplication(new UrbanSketchers.App());

            Xamarin.FormsMaps.Init(BingMapsKey);
        }

        public async Task<bool> AuthenticateAsync()
        {
            try
            {
                _user = await SketchManager.DefaultManager.CurrentClient.LoginAsync(MobileServiceAuthenticationProvider.Facebook, "urbansketchers");

                if (_user != null)
                {
                    return true;
                }
            }
            catch(Exception e)
            {
            }

            return false;
        }
    }
}