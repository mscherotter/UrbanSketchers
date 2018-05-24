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
        /// <summary>
        /// the location Id
        /// </summary>
        public enum LocationId
        {
            /// <summary>
            /// Pictures folder
            /// </summary>
            Pictures,
            /// <summary>
            /// 3D Models folder
            /// </summary>
            Models3D,

            /// <summary>
            /// Documents library
            /// </summary>
            Documents
        }

        /// <summary>
        /// The view mode
        /// </summary>
        public enum ViewMode
        {
            /// <summary>
            /// List view
            /// </summary>
            List,
            /// <summary>
            /// Thumbnail view
            /// </summary>
            Thumbnail
        }

        /// <summary>
        /// Gets the singleton current file picker service
        /// </summary>
        public static FilePickerService Current { get; set; }

        /// <summary>
        /// Pick a single file
        /// </summary>
        /// <param name="locationId">the starting location Id</param>
        /// <param name="viewMode">the view mode</param>
        /// <param name="fileTypeFilter">the file type filter</param>
        /// <returns>an async task with the file data</returns>
        public abstract Task<FileData> PickOpenFileAsync(
            LocationId locationId,
            ViewMode viewMode,
            IEnumerable<string> fileTypeFilter);

        public abstract Task<bool> PickSaveFileAsync(
            FileData fileData, LocationId locationId);
    }
}