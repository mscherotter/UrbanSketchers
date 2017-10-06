using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RootPage : MasterDetailPage
    {
        public RootPage()
        {
            InitializeComponent();
            //MasterPage.ListView.ItemSelected += ListView_ItemSelected;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as RootPageMenuItem;
            if (item == null)
                return;

            var page = (Page)Activator.CreateInstance(item.TargetType);
            page.Title = item.Title;

            Detail = new NavigationPage(page);
            IsPresented = false;

            //MasterPage.ListView.SelectedItem = null;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            var mapPage = new MapPage();

            var navigation = Detail as NavigationPage;
            var homePage = navigation.Navigation.NavigationStack.First();

            navigation.Navigation.InsertPageBefore(mapPage, homePage);

            await navigation.PopToRootAsync(false);
        }
    }
}