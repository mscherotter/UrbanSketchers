using System;
using UrbanSketchers.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Views
{
    /// <summary>
    ///     Edit sketch view
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditSketchView : Grid
    {
        /// <summary>
        ///     Initializes a new instance of the EditSketchView class.
        /// </summary>
        public EditSketchView()
        {
            InitializeComponent();
            this.PropertyChanged += EditSketchView_PropertyChanged;
        }

        private void EditSketchView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (BindingContext is Sketch sketch)
            {
                AddButton.Text = string.IsNullOrWhiteSpace(sketch.Id) ? "Add" : "Update";
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
            if (BindingContext is Sketch sketch)
            {
                if (string.IsNullOrWhiteSpace(sketch.Title)) return;

                await SketchManager.DefaultManager.SaveAsync(sketch);

                IsVisible = false;

                SketchSaved?.Invoke(this, new EventArgs());
            }
        }

        private void OnCancel(object sender, EventArgs e)
        {
            Canceled?.Invoke(this, new EventArgs());

            IsVisible = false;
        }
    }
}