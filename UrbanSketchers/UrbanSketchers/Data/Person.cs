using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanSketchers.Data
{
    public class Person : BaseDataObject
    {
        public string Name { get; set; }

        public string UserId { get; set; }

        public string ImageUrl { get; set; }

        public string PublicUrl { get; set; }

        /// <summary>
        /// Gets or sets the number of sketches for the person
        /// </summary>
        public int SketchCount { get; set; }
    }
}
