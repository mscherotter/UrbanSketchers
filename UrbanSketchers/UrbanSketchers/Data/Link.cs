namespace UrbanSketchers.Data
{
    /// <summary>
    ///     a link in the about page
    /// </summary>
    public class Link : BaseDataObject
    {
        /// <summary>
        ///     Gets or sets the title of the link
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Gets or sets the details of the link
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        ///     Gets or sets the link URL
        /// </summary>
        public string Url { get; set; }
    }
}