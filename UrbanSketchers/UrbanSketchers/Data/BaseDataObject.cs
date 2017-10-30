using System;
using UrbanSketchers.Helpers;

namespace UrbanSketchers.Data
{
    /// <summary>
    /// Base data object
    /// </summary>
    public class BaseDataObject : ObservableObject
    {
        /// <summary>
        /// Id for item
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Azure created at time stamp
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Azure UpdateAt timestamp for online/offline sync
        /// </summary>
        public DateTimeOffset UpdatedAt { get; set; }

        /// <summary>
        /// Azure version for online/offline sync
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the item was deleted
        /// </summary>
        public bool Deleted { get; set; }
    }
}
