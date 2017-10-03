using System;
using System.Threading.Tasks;
using UrbanSketchers.Data;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SketchPage
    {
        public SketchPage()
        {
            InitializeComponent();
        }

        public string SketchId { get; internal set; }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            var sketch = await SketchManager.DefaultManager.GetSketchAsync(SketchId);

            BindingContext = sketch;
        }

        private async void OnEdit(object sender, EventArgs e)
        {
            if (SketchManager.DefaultManager.CurrentClient.CurrentUser == null)
            {
                var authenticated = await App.Authenticator.AuthenticateAsync();

                if (!authenticated)
                {
                    return;
                }
            }

            EditSketch.IsVisible = true;
        }

        private async void OnRefresh(object sender, EventArgs e)
        {
            await RefreshAsync();
        }

        private async void OnTappedName(object sender, EventArgs e)
        {
            if (BindingContext is Sketch sketch)
            {
                await Navigation.PushAsync(new PersonPage
                {
                    PersonId = sketch.CreatedBy
                });
            }
        }
    }
}