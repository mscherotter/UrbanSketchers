namespace UrbanSketchers.Data
{
    /// <summary>
    ///     Urban Sketcher
    /// </summary>
    public class Person : BaseDataObject
    {
        /// <summary>
        ///     Gets or sets the name of the user
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the user Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        ///     Gets or sets the image Ur
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        ///     Gets or sets the public URL of the user
        /// </summary>
        public string PublicUrl { get; set; }

        /// <summary>
        ///     Gets or sets the number of sketches for the person
        /// </summary>
        public int SketchCount { get; set; }

        /// <summary>
        ///     Gets the account provider
        /// </summary>
        public string AccountProvider { get; set; }
    }
}