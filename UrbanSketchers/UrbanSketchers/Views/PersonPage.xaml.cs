using UrbanSketchers.Data;
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

            var sketches = await SketchManager.DefaultManager.GetSketchsAsync(PersonId);

            Sketches.ItemsSource = sketches;
        }

        private async void OnSketchSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is Sketch sketch)
            {
                // This won't work because the Xamarain x:Name "Image" isn't put onto the UWP Xaml Image
                //var data = new Controls.PrepareConnectedAnimationData("image2", sketch, "Sketch");

                //ConnectedAnimations.SetPrepareConnectedAnimation(Sketches, data);

                await Navigation.PushAsync(new SketchPage
                {
                    SketchId = sketch.Id
                }, true);
            }
        }
    }
}