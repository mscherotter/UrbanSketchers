using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanSketchers.Models
{
    public class Sketch : BaseDataObject
    {
        public string Title { get; set; }

        public DateTime CreationDate { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string ImageUrl { get; set; }

        public string CreatedBy { get; set; }
    }
}
