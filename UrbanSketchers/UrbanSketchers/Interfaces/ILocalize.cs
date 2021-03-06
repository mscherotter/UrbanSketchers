﻿using System;
using System.Globalization;

namespace UrbanSketchers.Interfaces
{
    /// <summary>
    ///     Localize interface
    /// </summary>
    /// <remarks>
    ///     See <![CDATA[https://developer.xamarin.com/guides/xamarin-forms/advanced/localization/]]></remarks>
    public interface ILocalize
    {
        /// <summary>
        ///     Gets the current culture information
        /// </summary>
        /// <returns>the current culture info</returns>
        CultureInfo GetCurrentCultureInfo();

        /// <summary>
        ///     Sets the current locale
        /// </summary>
        /// <param name="ci">the culture information</param>
        void SetLocale(CultureInfo ci);
    }

    /// <summary>
    ///     Helper class for splitting locales like
    ///     iOS: ms_MY, gsw_CH
    ///     Android: in-ID
    ///     into parts so we can create a .NET culture (or fallback culture)
    /// </summary>
    public class PlatformCulture
    {
        /// <summary>
        ///     Initializes a new instance of the PlatformCulture class
        /// </summary>
        /// <param name="platformCultureString"></param>
        public PlatformCulture(string platformCultureString)
        {
            if (string.IsNullOrEmpty(platformCultureString))
                throw new ArgumentException("Expected culture identifier",
                    nameof(platformCultureString)); // in C# 6 use nameof(platformCultureString)

            PlatformString = platformCultureString.Replace("_", "-"); // .NET expects dash, not underscore
            var dashIndex = PlatformString.IndexOf("-", StringComparison.Ordinal);
            if (dashIndex > 0)
            {
                var parts = PlatformString.Split('-');
                LanguageCode = parts[0];
                LocaleCode = parts[1];
            }
            else
            {
                LanguageCode = PlatformString;
                LocaleCode = "";
            }
        }

        /// <summary>
        ///     Gets the platform string
        /// </summary>
        public string PlatformString { get; }

        /// <summary>
        ///     Gets the language code
        /// </summary>
        public string LanguageCode { get; }

        /// <summary>
        ///     Gets the locale code
        /// </summary>
        public string LocaleCode { get; }

        /// <summary>
        ///     Converts the PlatformString
        /// </summary>
        /// <returns>the PlatformString</returns>
        public override string ToString()
        {
            return PlatformString;
        }
    }
}