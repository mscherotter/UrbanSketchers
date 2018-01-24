using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Pages
{
    /// <summary>
    /// Edit sketch page
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditSketchPage : IEditSketchPage
    {
        /// <summary>
        /// Initializes a new instance of the EditSketchPage class.
        /// </summary>
        public EditSketchPage()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Gets or sets the sketch Id
        /// </summary>
        public string SketchId { get; set; }

        /// <summary>
        /// Load the sketch
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var sketch = await SketchManager.DefaultManager.GetSketchAsync(SketchId);

            if (sketch != null)
            {
                var client = new System.Net.Http.HttpClient();
                var stream = await client.GetStreamAsync(sketch.ImageUrl);

                await EditView.LoadImageStreamAsync(stream);
            }

            BindingContext = sketch;
        }

        private async void OnSketchSaved(object sender, object e)
        {
            await Navigation.PopModalAsync(true);
        }

        private async void OnCanceled(object sender, System.EventArgs e)
        {
            await Navigation.PopModalAsync(true);
        }
    }
}