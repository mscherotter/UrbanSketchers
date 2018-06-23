using System;
using System.Collections.Generic;
using System.Text;

namespace UrbanSketchers.Data
{
    public class Resource
    {
        public string name { get; set; }
        public Point point { get; set; }

        public Address address { get; set; }
    }
}
