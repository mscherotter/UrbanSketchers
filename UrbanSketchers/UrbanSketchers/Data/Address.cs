using Newtonsoft.Json;

namespace UrbanSketchers.Data
{
    /// <summary>
    /// Location search address
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Gets or sets the admin district
        /// </summary>
        [JsonProperty("adminDistrict")] public string AdminDistrict { get; set; }

        /// <summary>
        /// Gets or sets the admin district 2
        /// </summary>
        [JsonProperty("adminDistrict2")] public string AdminDistrict2 { get; set; }

        /// <summary>
        /// Gets or sets the country region
        /// </summary>
        [JsonProperty("countryRegion")] public string CountryRegion { get; set; }

        /// <summary>
        /// Gets or sets the formatted address
        /// </summary>
        [JsonProperty("formattedAddress")] public string FormattedAddress { get; set; }
    }
}