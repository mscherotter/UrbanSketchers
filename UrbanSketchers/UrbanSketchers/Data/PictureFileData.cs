using System;
using System.Collections.Generic;
using System.Text;
using Plugin.FilePicker.Abstractions;

namespace UrbanSketchers.Data
{
    public class PictureFileData : FileData
    {
        public string Title { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
