using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrbanSketchers.Data;
using Xamarin.Forms;

namespace UrbanSketchers.ViewModels
{
    public interface IEditSketchPageViewModel
    {
        ISketch Sketch { get; set; }

        Task<ImageSource> LoadImageStreamAsync(Stream imageStream);
        Task<ImageSource> SelectFileAsync();
        Task<bool> AddAsync();
    }
}
