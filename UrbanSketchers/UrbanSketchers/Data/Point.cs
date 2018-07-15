using Newtonsoft.Json;

namespace UrbanSketchers.Data
{
    /// <summary>
    ///     Location Search result point
    /// </summary>
    public class Point
    {
        /// <summary>
        ///     Gets or sets the point type
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        ///     gets or sets the latitude/longitude coordinates
        /// </summary>
        [JsonProperty("coordinates")]
        public double[] Coordinates { get; set; }
    }
}