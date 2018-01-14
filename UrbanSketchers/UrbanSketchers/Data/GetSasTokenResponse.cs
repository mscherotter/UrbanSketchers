namespace UrbanSketchers.Data
{
    /// <summary>
    ///     Get SAS token response
    /// </summary>
    public class GetSasTokenResponse
    {
        /// <summary>
        ///     Gets or sets the SAS token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        ///     Gets or sets the SAS URL
        /// </summary>
        public string Uri { get; set; }
    }
}