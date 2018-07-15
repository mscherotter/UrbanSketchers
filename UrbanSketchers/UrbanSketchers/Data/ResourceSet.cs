using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace UrbanSketchers.Data
{
    /// <summary>
    /// Location search resource set
    /// </summary>
    public class ResourceSet
    {
        /// <summary>
        /// Gets or sets the resources
        /// </summary>
        [JsonProperty("resources")]
        public Resource[] Resources { get; set; }
    }
}
