using Newtonsoft.Json;

namespace UrbanSketchers.Data
{
    /// <summary>
    /// Location Search result resource
    /// </summary>
    public class Resource
    {
        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("point")] public Point Point { get; set; }

        [JsonProperty("address")] public Address Address { get; set; }
    }
}