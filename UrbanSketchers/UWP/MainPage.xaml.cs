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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private const string BingMapsKey =
                "B6p5hudPVN61Ykpp6D7W~JW-lf-G0P7wmsDcDrMWFuw~AsZdr0PHfOeWe9qmPtHDbuONPySTrgN47oWYdvD84J67bvxcMbXDQEnZCz6XWwR1";
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new UrbanSketchers.App());

            Xamarin.FormsMaps.Init(BingMapsKey);
        }
    }
}