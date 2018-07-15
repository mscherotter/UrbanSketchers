using Xamarin.Forms;

namespace UrbanSketchers.Interfaces
{
    /// <summary>
    /// Picture page interface
    /// </summary>
    public interface IPicturePage
    {
        /// <summary>
        /// Gets or sets the image source
        /// </summary>
        ImageSource ImageSource { get; set; }
    }
}