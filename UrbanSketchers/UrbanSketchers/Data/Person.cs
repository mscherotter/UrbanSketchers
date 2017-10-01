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

        public string ImageUrl { get; set; }

        public string PublicUrl { get; set; }
    }
}
