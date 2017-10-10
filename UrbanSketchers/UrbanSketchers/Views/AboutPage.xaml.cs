using Plugin.Share;
using UrbanSketchers.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();

            Links = new[]
            {
                new Link
                {
                    Title = "The Urban Sketches Map",
                    Details = "See the Urban sketch map on the web at urbansketchers.azurewebsites.net",
                    Url="http://urbansketchers.azurewebsites.net"
                },
                new Link
                {
                    Title = "About Michael Scherotter",
                    Details = "Learn about Michael Scherotter, the creator of Urban Sketchers on his website charette.com.",
                    Url = "http://charette.com"
                },
                new Link
                {
                    Title = "Source Code",
                    Details = "Contribute to the code for Urban Sketchers on GitHub.  Urban Sketches is built using Xamarin Forms and I need a little help with completing and publishing the iOS and Android version.",
                    Url = "https://github.com/mscherotter/UrbanSketchers"
                },
                new Link
                {
                    Title = "Translate",
                    Details = "Help translate Urban Sketchers to your language.",
                    Url =
                        "mailto:synergist@outlook.com?subject=Translate Urban Sketchers&body=I want to help translate the Urban Sketchers app to ...."
                },
                new Link
                {
                    Title = "Urban Sketchers Facebook Group",
                    Details = "Join the Urban Sketchers group on Facebook.",
                    Url = "https://www.facebook.com/groups/urbansketchers/"
                },
                new Link
                {
                    Title = "Twitter",
                    Details = "Follow Michael Scherotter (@Synergist) on Twitter",
                    Url = "https://twitter.com/synergist"
                }
            };

            BindingContext = this;
        }

        /// <summary>
        ///     Gets the links
        /// </summary>
        public Link[] Links { get; }

        private async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (CrossShare.IsSupported && e.SelectedItem is Link link)
                await CrossShare.Current.OpenBrowser(link.Url);
        }
    }
}