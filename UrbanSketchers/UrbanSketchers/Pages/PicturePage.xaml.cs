using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Pages
{
    /// <summary>
    /// Picture page
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PicturePage
    {
        /// <summary>
        /// Initializes a new instance of the PicturePage class.
        /// </summary>
        public PicturePage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the image source
        /// </summary>
        public ImageSource ImageSource
        {
            get => Image.Source;
            set => Image.Source = value;
        }

        /// <summary>
        /// Start the connected animation when appearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            Image.StartConnectedAnimation("Image");
        }
    }
}