using Newtonsoft.Json;

namespace UrbanSketchers.Data
{
    /// <summary>
    ///     Location response
    /// </summary>
    public class LocationResponse
    {
        /// <summary>
        ///     Gets or sets the resource sets
        /// </summary>
        [JsonProperty("resourceSets")]
        public ResourceSet[] ResourceSets { get; set; }
    }
}