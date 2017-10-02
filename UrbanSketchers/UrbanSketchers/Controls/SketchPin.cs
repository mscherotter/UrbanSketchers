using System;
using Xamarin.Forms.Maps;

namespace UrbanSketchers.Controls
{
    public class SketchPin
    {
        public Pin Pin { get; set; }

        //public string Id { get; set; }

        public string Url { get; set; }

        public event EventHandler Clicked;

        public void InvokeClick()
        {
            Clicked?.Invoke(this, new EventArgs());
        }
    }
}