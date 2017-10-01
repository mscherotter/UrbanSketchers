using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using UrbanSketchers.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PeoplePage : ContentPage
    {
        public ObservableCollection<Person> Items { get; set; }

        public PeoplePage()
        {
            InitializeComponent();

            Items = new ObservableCollection<Person>
            {
                new Person { Name = "Michael Scherotter"},
                new Person { Name = "Leonardo Da Vinci"},
                new Person { Name = "Le Corbusier"},
                new Person { Name = "Frank Lloyd Wright"}
            };

            BindingContext = this;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            await RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            var people = await SketchManager.DefaultManager.GetPeopleAsync();

            Items.SetRange(people);
        }

        async void Handle_ItemTapped(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}