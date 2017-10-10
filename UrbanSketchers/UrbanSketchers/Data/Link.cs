using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanSketchers.Data
{
    public class Link : BaseDataObject
    {
        public string Title { get; set; }
        public string Details { get; set; }
        public string Url { get; set; }
    }
}
