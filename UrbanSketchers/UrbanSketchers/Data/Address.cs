using System;
using System.Collections.Generic;
using System.Text;

namespace UrbanSketchers.Data
{
    public class Address
    {
        public string adminDistrict { get; set; }
        public string adminDistrict2 { get; set; }
        public string countryRegion { get; set; }

        public string formattedAddress { get; set; }

    }
}
