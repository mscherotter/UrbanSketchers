using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using UrbanSketchers.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers
{
    /// <summary>
    ///     Translate extension
    /// </summary>
    /// <remarks>
    ///     See <![CDATA[https://developer.xamarin.com/guides/xamarin-forms/advanced/localization/]]></remarks>
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        private const string ResourceId = "UrbanSketchers.Properties.Resources";
        private readonly CultureInfo _ci;

        /// <summary>
        ///     Initializes a new instance of the TranslateExtension class.
        /// </summary>
        public TranslateExtension()
        {
            if (Device.RuntimePlatform == Device.iOS || Device.RuntimePlatform == Device.Android)
                _ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
        }

        /// <summary>
        ///     Gets or sets the resource id
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///     Provide a value to translate
        /// </summary>
        /// <param name="serviceProvider">the service provider</param>
        /// <returns>the translated string</returns>
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
                return string.Empty;

            //var currentdcuc = CultureInfo.DefaultThreadCurrentUICulture;
            //var currentuic = CultureInfo.CurrentUICulture;
            //CultureInfo.DefaultThreadCurrentUICulture = _ci;
            //CultureInfo.CurrentUICulture = _ci;

            var resMgr = new ResourceManager(ResourceId, typeof(TranslateExtension).GetTypeInfo().Assembly);


            var translation = resMgr.GetString(Text, _ci);

            if (translation == null)
            {
#if DEBUG
                throw new ArgumentException(
                    $"Key '{Text}' was not found in resources '{ResourceId}' for culture '{_ci.Name}'.",
                    nameof(Text));
#else
                translation = Text; // returns the key, which GETS DISPLAYED TO THE USER
#endif
            }

            return translation;
        }
    }
}