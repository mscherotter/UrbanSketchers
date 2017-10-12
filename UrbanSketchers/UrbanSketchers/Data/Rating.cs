namespace UrbanSketchers.Data
{
    /// <summary>
    ///     A sketch rating
    /// </summary>
    public class Rating : BaseDataObject
    {
        public string SketchId { get; set; }

        public string PersonId { get; set; }

        public string Comment { get; set; } = string.Empty;

        public bool IsHeart { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the rating is a violation of terms
        /// </summary>
        public bool IsViolation { get; set; }

        /// <summary>
        ///     Gets or sets the Person.Name
        /// </summary>
        public string PersonName { get; set; }
    }
}