using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UrbanSketchers.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PeoplePage
    {
        public PeoplePage()
        {
            InitializeComponent();

            Items = new ObservableCollection<Person>();

            BindingContext = this;
        }

        public ObservableCollection<Person> Items { get; set; }

        /// <summary>
        /// Refresh the people
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                await RefreshAsync();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private async Task RefreshAsync()
        {
            var people = await SketchManager.DefaultManager.GetPeopleAsync();

            Items.SetRange(people);
        }

        private async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Person person)
                await Navigation.PushAsync(new SketchesPage
                {
                    PersonId = person.Id
                }, true);
        }
    }
}