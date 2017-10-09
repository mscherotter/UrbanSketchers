using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using UrbanSketchers.Data;
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

        public bool IsMapVisible
        {
            get { return Map.IsVisible; }
            set { Map.IsVisible = value; }
        }



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
        
        /// <summary>
        ///     Sketch saved event handler
        /// </summary>
        public event EventHandler SketchSaved;

        /// <summary>
        ///     Cancel event handler
        /// </summary>
        public event EventHandler Canceled;


        private async void OnAdd(object sender, EventArgs e)
        {
            ImageUrlEntry.IsEnabled = true;

            if (BindingContext is Sketch sketch)
            {
                if (string.IsNullOrWhiteSpace(sketch.Title)) return;

                if (_fileData == null && string.IsNullOrWhiteSpace(sketch.ImageUrl)) return;

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
                    }

                    using (var thumbnailStream =
                        await SketchManager.DefaultManager.ThumbnailGenerator.CreateThumbnailAsync(_fileData.DataArray))
                    {
                        var thumbnailFilename = string.Format(
                            CultureInfo.InvariantCulture,
                            "{0}-t{1}",
                            guid,
                            Path.GetExtension(_fileData.FileName));

                        sketch.ThumbnailUrl =
                            await SketchManager.DefaultManager.UploadAsync(thumbnailFilename, thumbnailStream);
                    }
                }

                await SketchManager.DefaultManager.SaveAsync(sketch);

                IsVisible = false;

                SketchSaved?.Invoke(this, new EventArgs());

                ClearFileData();
            }
        }

        private void OnCancel(object sender, EventArgs e)
        {
            ImageUrlEntry.IsEnabled = true;

            ClearFileData();

            Canceled?.Invoke(this, new EventArgs());

            IsVisible = false;
        }

        private void ClearFileData()
        {
            FilenameLabel.Text = string.Empty;

            RemoveFileButton.IsVisible = false;

            _fileData = null;
        }

        private async void OnSelectFile(object sender, EventArgs e)
        {
            _fileData = await CrossFilePicker.Current.PickFile();

            if (_fileData != null)
            {
                FilenameLabel.Text = _fileData.FileName;
                ImageUrlEntry.IsEnabled = false;
                RemoveFileButton.IsVisible = true;

                Image.Source = new StreamImageSource
                {
                    Stream = GetImageStream
                };
            }
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

            ImageUrlEntry.IsEnabled = true;
            FilenameLabel.Text = string.Empty;

            RemoveFileButton.IsVisible = false;
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