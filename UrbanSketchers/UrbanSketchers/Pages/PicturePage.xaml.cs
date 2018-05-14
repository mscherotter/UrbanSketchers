using UrbanSketchers.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Pages
{
    /// <summary>
    /// Picture page
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PicturePage : IPicturePage
    {
        /// <summary>
        /// Initializes a new instance of the PicturePage class.
        /// </summary>
        public PicturePage()
        {
            InitializeComponent();

            Image.PropertyChanged += Image_PropertyChanged;
        }

        private void Image_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Image.Width) || e.PropertyName == nameof(Image.Height))
            {
                if (Image.Width * Image.Height > 4)
                {
                    Image.StartConnectedAnimation("Image");
                }
            }
        }

        /// <summary>
        /// Gets or sets the image source
        /// </summary>
        public ImageSource ImageSource
        {
            get => Image.Source;
            set => Image.Source = value;
        }
    }
}