using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrbanSketchers;
using UWP.Renderers;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(ListView), typeof(ConnectedAnimationSourceListViewRenderer))]

namespace UWP.Renderers
{
    /// <summary>
    /// Connected animation source list view renderer
    /// </summary>
    public class ConnectedAnimationSourceListViewRenderer : ListViewRenderer
    {
        /// <summary>
        /// Element property changed
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the property chagned event arguments</param>
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "PrepareConnectedAnimation")
            {
                var data = ConnectedAnimations.GetPrepareConnectedAnimation(Element);

                if (data != null && ApiInformation.IsMethodPresent("Windows.UI.Xaml.Controls.ListViewBase", "PrepareConnectedAnimation"))
                {
                    List.PrepareConnectedAnimation(data.Key, data.Item, data.ElementName);
                }
            }
        }
    }
}
