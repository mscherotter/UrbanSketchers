using System;
using System.ComponentModel;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using UrbanSketchers.Data;
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

        private void EditSketchView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (BindingContext is Sketch sketch)
                AddButton.Text = string.IsNullOrWhiteSpace(sketch.Id) ? "Add" : "Update";
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
                    sketch.ImageUrl =
                        await SketchManager.DefaultManager.UploadAsync(_fileData.FileName, _fileData.DataArray);

                await SketchManager.DefaultManager.SaveAsync(sketch);

                IsVisible = false;

                SketchSaved?.Invoke(this, new EventArgs());

                _fileData = null;
            }
        }

        private void OnCancel(object sender, EventArgs e)
        {
            ImageUrlEntry.IsEnabled = true;

            _fileData = null;

            Canceled?.Invoke(this, new EventArgs());

            IsVisible = false;
        }

        private async void OnSelectFile(object sender, EventArgs e)
        {
            _fileData = await CrossFilePicker.Current.PickFile();

            if (_fileData != null)
            {
                FilenameLabel.Text = _fileData.FileName;
                ImageUrlEntry.IsEnabled = false;
                RemoveFileButton.IsVisible = true;
            }
        }

        private void OnRemoveFile(object sender, EventArgs e)
        {
            _fileData = null;

            ImageUrlEntry.IsEnabled = true;
            FilenameLabel.Text = string.Empty;

            RemoveFileButton.IsVisible = false;
        }
    }
}