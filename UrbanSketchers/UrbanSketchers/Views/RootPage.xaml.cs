using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RootPage
    {
        public RootPage()
        {
            InitializeComponent();
            //MasterPage.ListView.ItemSelected += ListView_ItemSelected;
        }

        //private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        //{
        //    var item = e.SelectedItem as RootPageMenuItem;
        //    if (item == null)
        //        return;

        //    var page = (Page)Activator.CreateInstance(item.TargetType);
        //    page.Title = item.Title;

        //    Detail = new NavigationPage(page);
        //    IsPresented = false;

        //    //MasterPage.ListView.SelectedItem = null;
        //}

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var mapPage = new MapPage();

            if (Detail is NavigationPage navigation)
            {
                var homePage = navigation.Navigation.NavigationStack.First();

                navigation.Navigation.InsertPageBefore(mapPage, homePage);

                await navigation.PopToRootAsync(false);
            }
        }
    }
}