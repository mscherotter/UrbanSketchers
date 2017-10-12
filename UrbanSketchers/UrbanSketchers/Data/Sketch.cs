using System;

namespace UrbanSketchers.Data
{
    /// <summary>
    ///     An urban sketch
    /// </summary>
    public class Sketch : BaseDataObject
    {
        private string _title;
        private string _address = string.Empty;
        private string _description = string.Empty;
        private string _imageUrl = string.Empty;
        private string _thumbnailUrl = string.Empty;
        private double _latitude;
        private double _longitude;

        /// <summary>
        ///     Gets or sets the title of the sketch
        /// </summary>
        public string Title
        {
            get => _title; 
            set => SetProperty(ref _title, value);
        }

        /// <summary>
        ///     Gets or sets the description of the sketch
        /// </summary>
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        /// <summary>
        ///     Gets or sets the creation date of the sketch
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        ///     Gets or sets the latitude
        /// </summary>
        public double Latitude
        {
            get => _latitude;

            set => SetProperty(ref _latitude, value);
        }

        /// <summary>
        ///     Gets or sets the longitude
        /// </summary>
        public double Longitude
        {
            get => _longitude;

            set => SetProperty(ref _longitude, value);
        }

        /// <summary>
        ///     Gets or sets the Image Url
        /// </summary>
        public string ImageUrl
        {
            get => _imageUrl;

            set => SetProperty(ref _imageUrl, value);
        }

        /// <summary>
        ///     Gets or sets the Id of the <see cref="Person" /> who created it.
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        ///     Gets or sets the address
        /// </summary>
        public string Address
        {
            get => _address;

            set => SetProperty(ref _address, value);
        }
        /// <summary>
        /// Gets the name of the sketch's creator
        /// </summary>
        public string CreatedByName { get; set; }

        /// <summary>
        /// Gets or sets the custom indexing sector
        /// </summary>
        public int Sector { get; set; }

        /// <summary>
        /// Gets or sets the thumbnail Url
        /// </summary>
        public string ThumbnailUrl
        {
            get => _thumbnailUrl;
            set => SetProperty(ref _thumbnailUrl, value);
        }
    }
}