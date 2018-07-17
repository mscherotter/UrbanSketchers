using System;
using Plugin.FilePicker.Abstractions;

namespace UrbanSketchers.Data
{
    /// <summary>
    /// Picture file data
    /// </summary>
    public class PictureFileData : FileData
    {
        /// <summary>
        /// Gets or sets the picture title from EXIF
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the latitude
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// Gets or sets the creation date
        /// </summary>
        public DateTime CreationDate { get; set; }
    }
}
