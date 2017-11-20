using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.FilePicker.Abstractions;

namespace UrbanSketchers.Support
{
    /// <summary>
    /// File picker service
    /// </summary>
    public abstract class FilePickerService
    {
        public enum LocationId
        {
            Pictures,
            Models3D
        }

        public enum ViewMode
        {
            List,
            Thumbnail
        }

        public static FilePickerService Current { get; set; }

        public abstract Task<FileData> PickOpenFileAsync(
            LocationId locationId,
            ViewMode viewMode,
            IEnumerable<string> fileTypeFilter);
    }
}