using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Views
{
    /// <summary>
    /// Person page
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonPage
    {
        /// <summary>
        /// Initializes a new instance of the PersonPage class.
        /// </summary>
        public PersonPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the person Id
        /// </summary>
        public string PersonId { get; set; }

        /// <summary>
        /// Sets the binding context to the person when appearing
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            BindingContext = await SketchManager.DefaultManager.GetPersonAsync(PersonId);
        }
    }
}