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
    public partial class PersonPage : ContentPage
    {
        public PersonPage()
        {
            InitializeComponent();
        }

        public string PersonId { get; set; }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            BindingContext = await SketchManager.DefaultManager.GetPersonAsync(PersonId);
        }
    }
}