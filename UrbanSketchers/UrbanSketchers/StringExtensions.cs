namespace UrbanSketchers
{
    /// <summary>
    ///     String extensions
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        ///     Truncate a string
        /// </summary>
        /// <param name="value">the string</param>
        /// <param name="maxLength">the maximum length</param>
        /// <returns>the truncated string that is no longer than maxLength</returns>
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}