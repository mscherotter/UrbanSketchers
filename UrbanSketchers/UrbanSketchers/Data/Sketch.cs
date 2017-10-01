using System;

namespace UrbanSketchers.Data
{
    /// <summary>
    /// An urban sketch
    /// </summary>
    public class Sketch : BaseDataObject
    {
        /// <summary>
        /// Gets or sets the title of the sketch
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the creation date of the sketch
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the latitude
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the Image Url
        /// </summary>
        public string ImageUrl { get; set; }

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
