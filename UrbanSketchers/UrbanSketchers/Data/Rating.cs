using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanSketchers.Data
{
    /// <summary>
    /// A sketch rating
    /// </summary>
    public class Rating : BaseDataObject
    {
        public string SketchId { get; set; }

        public string UserId { get; set; }

        public string Comment { get; set; } = string.Empty;

        public bool IsHeart { get; set; }

        public bool IsViolation { get; set; }
    }
}
