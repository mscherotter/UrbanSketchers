using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using UrbanSketchers.Helpers;
using UrbanSketchers.Interfaces;
using UrbanSketchers.Support;
using Xamarin.Forms;

namespace UrbanSketchers.ViewModels
{
    public class EditSketchPageViewModel : ObservableObject, IEditSketchPageViewModel
    {
        private FileData _fileData;
        private ISketch _sketch;

        public bool CanAdd
        {
            get
            {
                if (_sketch == null) return false;

                if (string.IsNullOrWhiteSpace(_sketch.Title)) return false;

                return true;
            }
        }


        public ISketch Sketch
        {
            get => _sketch;
            set
            {
                if (SetProperty(ref _sketch, value))
                    if (value is INotifyPropertyChanged propertyChanged)
                        propertyChanged.PropertyChanged += PropertyChanged_PropertyChanged;
            }
        }

        public async Task<ImageSource> LoadImageStreamAsync(Stream imageStream)
        {
            if (imageStream.CanSeek)
            {
                _fileData = new FileData
                {
                    DataArray = new byte[imageStream.Length],
                    FileName = "Sketch.png"
                };

                imageStream.Read(_fileData.DataArray, 0, Convert.ToInt32(imageStream.Length));
            }
            else
            {
                var memoryStream = new MemoryStream();

                await imageStream.CopyToAsync(memoryStream);

                memoryStream.Seek(0, SeekOrigin.Begin);

                _fileData = new FileData
                {
                    DataArray = new byte[memoryStream.Length],
                    FileName = "Sketch.png"
                };

                memoryStream.Read(_fileData.DataArray, 0, Convert.ToInt32(memoryStream.Length));
            }

            return new StreamImageSource
            {
                Stream = GetImageStream
            };
        }

        public async Task<ImageSource> SelectFileAsync()
        {
            if (FilePickerService.Current == null)
                _fileData = await CrossFilePicker.Current.PickFile();
            else
                _fileData = await FilePickerService.Current.PickOpenFileAsync(
                    FilePickerService.LocationId.Pictures,
                    FilePickerService.ViewMode.Thumbnail,
                    new[] {".png", ".jpg"});

            if (_fileData == null)
                return null;

            return new StreamImageSource
            {
                Stream = GetImageStream
            };
        }

        public async Task<bool> AddAsync()
        {
            if (string.IsNullOrWhiteSpace(Sketch.Title)) return false;

            if (_fileData == null && string.IsNullOrWhiteSpace(Sketch.ImageUrl)) return false;

            ////if (ShouldUpload != null)
            ////    if (!await ShouldUpload(Sketch))
            ////        return false;

            if (_fileData != null)
            {
                var guid = Guid.NewGuid().ToString();

                var filename = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}{1}",
                    guid,
                    Path.GetExtension(_fileData.FileName));

                using (var stream = new MemoryStream(_fileData.DataArray))
                {
                    Sketch.ImageUrl =
                        await SketchManager.DefaultManager.UploadAsync(filename, stream);

                    Sketch.ThumbnailUrl = Sketch.ImageUrl.Replace("/sketches/", "/thumbnails/");
                }
            }

            try
            {
                await SketchManager.DefaultManager.SaveAsync(Sketch);
                //SketchSaved?.Invoke(this, new TypedEventArgs<ISketch>(sketch));
            }
            catch (Exception exception)
            {
                Debug.WriteLine("Error saving sketch: " + exception.Message);

                return false;
            }

            return true;
        }

        private Task<Stream> GetImageStream(CancellationToken arg)
        {
            return Task.Run(delegate
            {
                if (_fileData == null)
                    return null;

                Stream stream = new MemoryStream(_fileData.DataArray);

                return stream;
            }, arg);
        }

        private void PropertyChanged_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            OnPropertyChanged(nameof(CanAdd));
        }
    }
}