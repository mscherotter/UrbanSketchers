using UrbanSketchers.Interfaces;

namespace UrbanSketchers.Data
{
    /// <summary>
    ///     A sketch rating
    /// </summary>
    public class Rating : BaseDataObject, IRating
    {
        /// <summary>
        ///     Gets or sets the sketch Id
        /// </summary>
        public string SketchId { get; set; }

        /// <summary>
        ///     Gets or sets the person Id
        /// </summary>
        public string PersonId { get; set; }

        /// <summary>
        ///     Gets or sets the comment
        /// </summary>
        public string Comment { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the favorite/heart flag
        /// </summary>
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