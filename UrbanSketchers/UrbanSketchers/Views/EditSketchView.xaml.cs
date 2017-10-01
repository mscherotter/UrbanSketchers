using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrbanSketchers.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditSketchView : Grid
    {
        public EditSketchView()
        {
            InitializeComponent();
        }

        public event EventHandler SketchSaved;

        public event EventHandler Canceled;

        private async void OnAdd(object sender, EventArgs e)
        {
            if (BindingContext is Sketch sketch)
            {
                if (string.IsNullOrWhiteSpace(sketch.Title)) return;

                await SketchManager.DefaultManager.SaveSketchAsync(sketch);

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