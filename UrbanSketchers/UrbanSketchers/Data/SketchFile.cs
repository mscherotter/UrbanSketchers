using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanSketchers.Data
{
    public class SketchFile
    {
        public string BlobName { get; set; }
        public string Container { get; set; }

        public string Permissions { get; set; }
    }
}
