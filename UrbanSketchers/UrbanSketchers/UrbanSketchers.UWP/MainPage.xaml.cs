namespace UrbanSketchers.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            LoadApplication(new UrbanSketchers.App());

            Xamarin.FormsMaps.Init("B6p5hudPVN61Ykpp6D7W~JW-lf-G0P7wmsDcDrMWFuw~AsZdr0PHfOeWe9qmPtHDbuONPySTrgN47oWYdvD84J67bvxcMbXDQEnZCz6XWwR1");
        }
    }
}