using Xamarin.Forms;

namespace UrbanSketchers.Interfaces
{
    /// <summary>
    ///     Sketch comments page view model interface
    /// </summary>
    public interface ISketchCommentsPageViewModel
    {
        /// <summary>
        ///     Gets or sets the Sketch Id
        /// </summary>
        string SketchId { get; set; }

        /// <summary>
        /// Gets or sets the page
        /// </summary>
        Page Page { get; set; }
    }
}