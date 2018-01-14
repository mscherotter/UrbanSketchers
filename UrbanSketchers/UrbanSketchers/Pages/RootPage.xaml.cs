using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Pages
{
    /// <summary>
    ///     the root page
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RootPage
    {
        private bool _isFirst = true;

        /// <summary>
        ///     Initializes a new instance of the RootPage class.
        /// </summary>
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

        /// <summary>
        ///     Navigate to the map page
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_isFirst)
            {
                var mapPage = new MapPage();

                if (Detail is NavigationPage navigation)
                {
                    var homePage = navigation.Navigation.NavigationStack.First();

                    navigation.Navigation.InsertPageBefore(mapPage, homePage);

                    await navigation.PopToRootAsync(false);
                }

                _isFirst = false;
            }
        }
    }
}