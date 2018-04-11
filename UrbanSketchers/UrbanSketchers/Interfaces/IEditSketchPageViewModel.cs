using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace UrbanSketchers.Interfaces
{
    public interface IEditSketchPageViewModel
    {
        ISketch Sketch { get; set; }

        Task<ImageSource> LoadImageStreamAsync(Stream imageStream);
        Task<ImageSource> SelectFileAsync();
        Task<bool> AddAsync();
    }
}
