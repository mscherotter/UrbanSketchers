using System;

namespace UrbanSketchers.Interfaces
{
    /// <summary>
    ///     Rating interface
    /// </summary>
    public interface IRating
    {
        /// <summary>
        ///     Gets or sets the comment
        /// </summary>
        string Comment { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this is a TOS violation
        /// </summary>
        bool IsViolation { get; set; }

        /// <summary>
        ///     Gets or sets the Id
        /// </summary>
        string Id { get; set; }

        /// <summary>
        ///     Gets or sets the sketch Id
        /// </summary>
        string SketchId { get; set; }

        /// <summary>
        ///     Gets or sets the updated at date
        /// </summary>
        DateTimeOffset UpdatedAt { get; set; }

        /// <summary>
        ///     Gets or sets the person Id
        /// </summary>
        string PersonId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user Heart'ed the sketch
        /// </summary>
        bool IsHeart { get; set; }
    }
}