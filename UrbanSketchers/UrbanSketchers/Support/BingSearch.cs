using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using UrbanSketchers.Data;

namespace UrbanSketchers.Support
{
    /// <summary>
    ///     Bing search
    /// </summary>
    public class BingSearch
    {
        /// <summary>
        ///     search for a location with Bing Search
        /// </summary>
        /// <remarks><![CDATA[https://msdn.microsoft.com/en-us/library/ff701711.aspx]]></remarks>
        /// <param name="name">the name of the location</param>
        /// <returns>a list of resources</returns>
        public static List<Resource> LocationSearch(string name)
        {
            try
            {
                var query = Uri.EscapeUriString(name);

                if (string.IsNullOrWhiteSpace(query)) return null;

                var bingMapsKey =
                    "B6p5hudPVN61Ykpp6D7W~JW-lf-G0P7wmsDcDrMWFuw~AsZdr0PHfOeWe9qmPtHDbuONPySTrgN47oWYdvD84J67bvxcMbXDQEnZCz6XWwR1";

                var uriString =
                    $"http://dev.virtualearth.net/REST/v1/Locations?query={query}&includeNeighborhood=1&maxResults=20&key={bingMapsKey}";

                var requestUri = new Uri(uriString);

                var request = WebRequest.Create(requestUri);

                request.Method = "GET";
                request.ContentType = "application/json";

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    if (response == null) return null;

                    if (response.StatusCode != HttpStatusCode.OK) return null;

                    var stream = response.GetResponseStream();

                    if (stream == null) return null;

                    using (var reader = new StreamReader(stream))
                    {
                        var content = reader.ReadToEnd();

                        var locationResponse = JsonConvert.DeserializeObject<LocationResponse>(content);

                        if (locationResponse.ResourceSets == null) return null;

                        var items = from item in locationResponse.ResourceSets
                            from item2 in item.Resources
                            select item2;

                        return items.ToList();
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}