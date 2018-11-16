using System;
using System.Diagnostics.CodeAnalysis;

namespace UrbanSketchers.Interfaces
{
    /// <summary>
    ///     Sketch interface
    /// </summary>
    public interface ISketch
    {
        /// <summary>
        ///     Gets the sketch Id
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     Gets or sets the latitude
        /// </summary>
        double Latitude { get; set; }

        /// <summary>
        ///     Gets or sets the longitude
        /// </summary>
        double Longitude { get; set; }

        /// <summary>
        ///     Gets or sets the address of the sketch
        /// </summary>
        string Address { get; set; }

        /// <summary>
        ///     Gets or sets the title of the sketch
        /// </summary>
        string Title { get; set; }

        /// <summary>
        ///     Gets or sets the id of the sketcher
        /// </summary>
        string CreatedBy { get; set; }

        /// <summary>
        ///     Gets or sets the name of the sketcher
        /// </summary>
        string CreatedByName { get; set; }

        /// <summary>
        /// Gets or sets the thumbnail URL
        /// </summary>
        [SuppressMessage("Design", "CA1056", Justification = "Necessary for serialization")]        
        string ThumbnailUrl { get; set; }

        /// <summary>
        /// Gets or sets the sector
        /// </summary>
        int Sector { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets or sets the image URL
        /// </summary>
        [SuppressMessage("Design", "CA1056", Justification = "Necessary for serialization")]        
        string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the creation date
        /// </summary>
        DateTime CreationDate { get; set; }
    }
}