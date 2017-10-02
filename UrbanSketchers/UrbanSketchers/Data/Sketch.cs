using System;

namespace UrbanSketchers.Data
{
    /// <summary>
    /// An urban sketch
    /// </summary>
    public class Sketch : BaseDataObject
    {
        private double _latitude;
        private double _longitude;
        private string _imageUrl;
        /// <summary>
        /// Gets or sets the title of the sketch
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description of the sketch
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the creation date of the sketch
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the latitude
        /// </summary>
        public double Latitude
        {
            get
            {
                return _latitude;
            }

            set
            {
                SetProperty(ref _latitude, value);
            }
        }

        /// <summary>
        /// Gets or sets the longitude
        /// </summary>
        public double Longitude
        {
            get
            {
                return _longitude;
            }

            set
            {
                SetProperty(ref _longitude, value);
            }
        }

        /// <summary>
        /// Gets or sets the Image Url
        /// </summary>
        public string ImageUrl
        {
            get
            {
                return _imageUrl;
            }

            set
            {
                SetProperty(ref _imageUrl, value);
            }
        }

        /// <summary>
        /// Gets or sets the Id of the <see cref="Person"/> who created it.
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the address
        /// </summary>
        public string Address { get; set; }
    }
}
