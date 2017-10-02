using System;

namespace UrbanSketchers.Data
{
    /// <summary>
    ///     An urban sketch
    /// </summary>
    public class Sketch : BaseDataObject
    {
        private string _imageUrl;
        private double _latitude;
        private double _longitude;

        /// <summary>
        ///     Gets or sets the title of the sketch
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Gets or sets the description of the sketch
        /// </summary>
        public string Description { get; set; }

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
        public string Address { get; set; }

        public string CreatedByName { get; set; }
    }
}