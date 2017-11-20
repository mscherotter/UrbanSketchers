using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using UrbanSketchers.Data;
using UrbanSketchers.Support;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Views
{
    /// <summary>
    ///     Edit sketch view
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditSketchView
    {
        private FileData _fileData;

        /// <summary>
        ///     Initializes a new instance of the EditSketchView class.
        /// </summary>
        public EditSketchView()
        {
            InitializeComponent();
            PropertyChanged += EditSketchView_PropertyChanged;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the map is visible
        /// </summary>
        public bool IsMapVisible
        {
            get => Map.IsVisible;
            set => Map.IsVisible = value;
        }

        /// <summary>
        /// Gets or sets the image stream filled by a modal page (like the DrawingPage)
        /// </summary>
        public Stream ImageStream { get; set; }

        private async void EditSketchView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (BindingContext is Sketch sketch)
            {
                AddButton.Text = string.IsNullOrWhiteSpace(sketch.Id) ? Properties.Resources.Add : Properties.Resources.Update;
            }

            if (e.PropertyName == nameof(IsVisible) && IsVisible)
            {
                if (BindingContext is Sketch sketch2)
                {
                    var span = MapSpan.FromCenterAndRadius(new Position(sketch2.Latitude, sketch2.Longitude),
                        Distance.FromMiles(1.0));

                    await Task.Delay(TimeSpan.FromSeconds(0.5));

                    Map.SetVisibleRegion(span);
                }
            }
        }

        internal void LoadImageStream(Stream imageStream)
        {
            _fileData = new FileData
            {
                DataArray = new byte[imageStream.Length],
                FileName = "Sketch.png"
            };

            imageStream.Read(_fileData.DataArray, 0, Convert.ToInt32(imageStream.Length));

            LoadFileData();
        }

        /// <summary>
        ///     Sketch saved event handler
        /// </summary>
        public event EventHandler<TypedEventArgs<Sketch>> SketchSaved;

        /// <summary>
        ///     Cancel event handler
        /// </summary>
        public event EventHandler Canceled;

        /// <summary>
        /// should the sketch be uploaded?
        /// </summary>
        public Func<Sketch, Task<bool>> ShouldUpload;

        private async void OnAdd(object sender, EventArgs e)
        {
            //ImageUrlEntry.IsEnabled = true;

            if (BindingContext is Sketch sketch)
            {
                if (string.IsNullOrWhiteSpace(sketch.Title)) return;

                if (_fileData == null && string.IsNullOrWhiteSpace(sketch.ImageUrl)) return;

                if (ShouldUpload != null)
                {
                    if (!await ShouldUpload(sketch))
                    {
                        return;
                    }
                }

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
                        sketch.ImageUrl =
                            await SketchManager.DefaultManager.UploadAsync(filename, stream);

                        sketch.ThumbnailUrl = sketch.ImageUrl.Replace("/sketches/", "/thumbnails/");
                    }
                }

                try
                {

                    await SketchManager.DefaultManager.SaveAsync(sketch);
                    SketchSaved?.Invoke(this, new TypedEventArgs<Sketch>(sketch));
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine("Error saving sketch: " + exception.Message);

                    return;
                }

                IsVisible = false;

                ClearFileData();
            }
        }

        private void OnCancel(object sender, EventArgs e)
        {
            //ImageUrlEntry.IsEnabled = true;

            ClearFileData();

            Canceled?.Invoke(this, new EventArgs());

            IsVisible = false;
        }

        private void ClearFileData()
        {
            FilenameLabel.Text = string.Empty;

            RemoveFileButton.IsEnabled = false;

            Image.Source = null;

            _fileData = null;
        }

        private async void OnSelectFile(object sender, EventArgs e)
        {
            if (FilePickerService.Current == null)
            {
                _fileData = await CrossFilePicker.Current.PickFile();
            }
            else
            {
                _fileData = await FilePickerService.Current.PickOpenFileAsync(
                    FilePickerService.LocationId.Pictures,
                    FilePickerService.ViewMode.Thumbnail,
                    new[] {".png", ".jpg"});
            }

            if (_fileData != null)
            {
                LoadFileData();
            }
        }

        private void LoadFileData()
        {
            FilenameLabel.Text = _fileData.FileName;
            //ImageUrlEntry.IsEnabled = false;
            RemoveFileButton.IsEnabled = true;

            Image.Source = new StreamImageSource
            {
                Stream = GetImageStream
            };
        }

        private Task<Stream> GetImageStream(CancellationToken arg)
        {
            return Task.Run(delegate
            {
                if (_fileData == null)
                {
                    return null;
                }

                Stream stream = new MemoryStream(_fileData.DataArray);

                return stream;
            }, arg);
        }

        private void OnRemoveFile(object sender, EventArgs e)
        {
            _fileData = null;

            //ImageUrlEntry.IsEnabled = true;
            FilenameLabel.Text = string.Empty;

            RemoveFileButton.IsEnabled = false;
            Image.Source = null;
        }

        private void OnMapPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Map.IsVisible && BindingContext is Sketch && Map.IsFocused)
            {
                if (e.PropertyName == nameof(Map.VisibleRegion))
                {
                    UpdateLocation();
                }
            }
        }

        private void UpdateLocation()
        {
            if (BindingContext is Sketch sketch)
            {
                sketch.Latitude = Map.VisibleRegion.Center.Latitude;
                sketch.Longitude = Map.VisibleRegion.Center.Longitude;
            }
        }
    }
}