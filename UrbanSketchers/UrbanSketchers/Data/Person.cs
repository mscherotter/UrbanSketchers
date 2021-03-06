﻿using System.Diagnostics.CodeAnalysis;
using UrbanSketchers.Interfaces;

namespace UrbanSketchers.Data
{
    /// <summary>
    ///     Urban Sketcher
    /// </summary>
    public class Person : BaseDataObject, IPerson
    {
        /// <summary>
        ///     Gets or sets the name of the user
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the user Id from Azure
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        ///     Gets or sets the image Ur
        /// </summary>
        [SuppressMessage("Design", "CA1056", Justification = "Necessary for serialization")]        
        public string ImageUrl { get; set; }

        /// <summary>
        ///     Gets or sets the public URL of the user
        /// </summary>
        [SuppressMessage("Design", "CA1056", Justification = "Necessary for serialization")]        
        public string PublicUrl { get; set; }

        /// <summary>
        ///     Gets or sets the number of sketches for the person
        /// </summary>
        public int SketchCount { get; set; }

        /// <summary>
        ///     Gets the account provider
        /// </summary>
        public string AccountProvider { get; set; }

        /// <summary>
        /// Gets a value indicating whether the user is a system administrator.
        /// </summary>
        public bool IsAdministrator { get; set; }
    }
}