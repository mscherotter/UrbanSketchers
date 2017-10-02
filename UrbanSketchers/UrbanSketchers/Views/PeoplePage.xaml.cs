using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UrbanSketchers.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PeoplePage : ContentPage
    {
        public PeoplePage()
        {
            InitializeComponent();

            Items = new ObservableCollection<Person>();

            BindingContext = this;
        }

        public ObservableCollection<Person> Items { get; set; }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            var people = await SketchManager.DefaultManager.GetPeopleAsync();

            Items.SetRange(people);
        }

        private async void Handle_ItemTapped(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            if (e.SelectedItem is Person person)
            {
                await this.Navigation.PushAsync(new SketchesPage
                {
                    PersonId = person.Id
                }, true);
            }
            //await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView) sender).SelectedItem = null;
        }
    }
}