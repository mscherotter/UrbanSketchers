using Autofac;
using UrbanSketchers.Data;
using UrbanSketchers.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Pages
{
    /// <summary>
    ///     Person page
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonPage
    {
        private readonly IPersonPageViewModel _viewModel;

        /// <summary>
        ///     Initializes a new instance of the PersonPage class.
        /// </summary>
        public PersonPage()
        {
            InitializeComponent();

            _viewModel = Core.Container.Current.Resolve<IPersonPageViewModel>();

            BindingContext = _viewModel;
        }

        /// <summary>
        ///     Gets or sets the person Id
        /// </summary>
        public string PersonId
        {
            get => _viewModel.PersonId;
            set => _viewModel.PersonId = value;
        }

        /// <summary>
        ///     Sets the binding context to the person when appearing
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await _viewModel.RefreshAsync();
        }

        private async void OnSketchSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is ISketch sketch)
            {
                var page = Core.Container.Current.Resolve<ISketchPage>();

                page.SketchId = sketch.Id;

                await Navigation.PushAsync(page as Page, true);
            }
        }
    }
}