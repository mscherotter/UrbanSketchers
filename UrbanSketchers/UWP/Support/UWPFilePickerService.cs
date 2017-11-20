using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Plugin.FilePicker.Abstractions;
using UrbanSketchers.Support;

namespace UWP.Support
{
    /// <summary>
    /// UWP File Picker Service
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class UWPFilePickerService : FilePickerService
    {
        private readonly Dictionary<LocationId, PickerLocationId> _locationIdDictionary =
            new Dictionary<LocationId, PickerLocationId>();

        private readonly Dictionary<ViewMode, PickerViewMode> _viewModeDictionary =
            new Dictionary<ViewMode, PickerViewMode>();

        /// <summary>
        /// Initializes a new instance of the UWPFilePickerService
        /// </summary>
        public UWPFilePickerService()
        {
            _locationIdDictionary[LocationId.Pictures] = PickerLocationId.PicturesLibrary;
            _locationIdDictionary[LocationId.Models3D] = PickerLocationId.Objects3D;

            _viewModeDictionary[ViewMode.List] = PickerViewMode.List;
            _viewModeDictionary[ViewMode.Thumbnail] = PickerViewMode.Thumbnail;
        }

        /// <summary>
        /// Pick a file to open
        /// </summary>
        /// <param name="locationId">the location Id</param>
        /// <param name="viewMode">the view mode</param>
        /// <param name="fileTypeFilter">the file type filter</param>
        /// <returns>an async task with the file data or null if the user cancels.</returns>
        public override async Task<FileData> PickOpenFileAsync(LocationId locationId, ViewMode viewMode,
            IEnumerable<string> fileTypeFilter)
        {
            var picker = new FileOpenPicker
            {
                SuggestedStartLocation = _locationIdDictionary[locationId],
                ViewMode = _viewModeDictionary[viewMode]
            };

            foreach (var item in fileTypeFilter)
                picker.FileTypeFilter.Add(item);

            var file = await picker.PickSingleFileAsync();

            if (file == null)
                return null;

            using (var stream = await file.OpenStreamForReadAsync())
            {
                var fileData = new FileData
                {
                    DataArray = new byte[stream.Length],
                    FileName = file.Name
                };

                var dataRead = 0;

                do
                {
                    dataRead += await stream.ReadAsync(fileData.DataArray, dataRead, Convert.ToInt32(stream.Length));

                } while (dataRead < stream.Length);

                return fileData;
            }
        }
    }

}
