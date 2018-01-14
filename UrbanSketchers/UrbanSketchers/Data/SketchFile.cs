namespace UrbanSketchers.Data
{
    /// <summary>
    /// A Sketch file
    /// </summary>
    public class SketchFile
    {
        /// <summary>
        /// Gets or sets the blob name
        /// </summary>
        public string BlobName { get; set; }

        /// <summary>
        /// Gets or sets the container
        /// </summary>
        public string Container { get; set; }

        /// <summary>
        /// Gets or sets the permissions
        /// </summary>
        public string Permissions { get; set; }
    }
}