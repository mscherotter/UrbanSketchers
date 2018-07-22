using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace UrbanSketchers.Interfaces
{
    /// <summary>
    /// Edit sketch page view model interface
    /// </summary>
    public interface IEditSketchPageViewModel
    {
        /// <summary>
        /// Gets or sets the sketch
        /// </summary>
        ISketch Sketch { get; set; }

        /// <summary>
        /// Gets the delete sketch command
        /// </summary>
        IDeleteSketchCommand DeleteSketchCommand {get; }

        /// <summary>
        /// Load an image stream
        /// </summary>
        /// <param name="imageStream">the image stream</param>
        /// <returns>an async task with the image source</returns>
        Task<ImageSource> LoadImageStreamAsync(Stream imageStream);
        
        /// <summary>
        /// Select a file
        /// </summary>
        /// <returns>an async task with an image source</returns>
        Task<ImageSource> SelectFileAsync();

        /// <summary>
        /// Add an image
        /// </summary>
        /// <returns>an async task with a boolean value</returns>
        Task<bool> AddAsync();
    }
}
