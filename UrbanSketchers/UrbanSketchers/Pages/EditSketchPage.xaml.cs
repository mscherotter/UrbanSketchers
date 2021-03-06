﻿using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using UrbanSketchers.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Pages
{
    /// <summary>
    ///     Edit sketch page
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditSketchPage : IEditSketchPage
    {
        private readonly IEditSketchPageViewModel _viewModel;
        private bool _initialized;


        /// <summary>
        ///     should the sketch be uploaded?
        /// </summary>
        public Func<ISketch, Task<bool>> ShouldUpload;
        private readonly ISketchManager _sketchManager;

        public EditSketchPage()
        {
            InitializeComponent();
        }
        /// <summary>
        ///     Initializes a new instance of the EditSketchPage class.
        /// </summary>
        /// <param name="viewModel">the view model interface</param>
        /// <param name="sketchManager">the sketch manager</param>
        public EditSketchPage(IEditSketchPageViewModel viewModel,
            ISketchManager sketchManager)
        {
            _sketchManager = sketchManager;

            InitializeComponent();

            _viewModel = viewModel;

            _viewModel.DeleteSketchCommand.Page = this;

            BindingContext = _viewModel;
        }

        /// <summary>
        ///     Gets or sets the sketch
        /// </summary>
        public ISketch Sketch
        {
            get => _viewModel.Sketch;
            set => _viewModel.Sketch = value;
        }

        /// <summary>
        ///     Gets or sets the sketch Id
        /// </summary>
        public string SketchId { get; set; }

        /// <summary>
        /// Gets or sets the map radius
        /// </summary>
        public Distance Radius { get;set;}

        /// <summary>
        /// Gets or sets the map type
        /// </summary>
        public MapType MapType { get;  set; } = MapType.Hybrid;

        /// <summary>
        ///     Load the sketch
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (string.IsNullOrWhiteSpace(SketchId))
            {
                var position = new Position(_viewModel.Sketch.Latitude, _viewModel.Sketch.Longitude);

                Map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Radius));
                Map.MapType = MapType;
            }
            else
            {
                var sketch = await _sketchManager.GetSketchAsync(SketchId);

                if (sketch != null)
                {
                    using (var client = new HttpClient())
                    {
                        var stream = await client.GetStreamAsync(sketch.ImageUrl);
                    
                        await LoadImageStreamAsync(stream);
                    }
                }

                _viewModel.Sketch = sketch;
            }

            _initialized = true;
        }

        internal async Task LoadImageStreamAsync(Stream imageStream)
        {
            Image.SetIsLoading(true);

            Image.Source = await _viewModel.LoadImageStreamAsync(imageStream);

            Image.SetIsLoading(false);
        }

        private void OnMapPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Map.VisibleRegion)) UpdateLocation();
        }

        private void UpdateLocation()
        {
            if (!_initialized) return;

            _viewModel.Sketch.Latitude = Map.VisibleRegion.Center.Latitude;
            _viewModel.Sketch.Longitude = Map.VisibleRegion.Center.Longitude;
        }

        private async void OnSelectFile(object sender, EventArgs e)
        {
            var imageSource = await _viewModel.SelectFileAsync();

            if (imageSource != null)
                Image.Source = imageSource;
        }


        private async void OnAdd(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.IsEnabled = false;

                if (await _viewModel.AddAsync())
                    await Navigation.PopModalAsync(true);

                button.IsEnabled = true;
            }
        }

        private async void OnCancel(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(true);
        }
    }
}