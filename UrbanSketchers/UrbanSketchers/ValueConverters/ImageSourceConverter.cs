using System;
using System.Globalization;
using PCLStorage;
using Xamarin.Forms;

namespace UrbanSketchers.ValueConverters
{
    /// <summary>
    ///     converts from a HTTP URI or a filename to an ImageSource
    /// </summary>
    public class ImageSourceConverter : IValueConverter
    {
        /// <summary>
        ///     If the value is a http or https URL return a UriImageSource, otherwise return a FileImageSource
        /// </summary>
        /// <param name="value">a string</param>
        /// <param name="targetType">an <see cref="ImageSource" /></param>
        /// <param name="parameter">the parameter is not used</param>
        /// <param name="culture">the culture is not used</param>
        /// <returns>a <see cref="UriImageSource" /> or a <see cref="FileImageSource" /></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string filename)
            {
                if (filename.StartsWith("http://") || filename.StartsWith("https://")) return new UriImageSource {Uri = new Uri(filename)};

                var fullPath = FileSystem.Current.LocalStorage.Path + @"\" + filename;

                return new FileImageSource {File = fullPath};
            }

            return null;
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="value">not used.</param>
        /// <param name="targetType">not used.</param>
        /// <param name="parameter">not used.</param>
        /// <param name="culture">not used.</param>
        /// <returns>not used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}