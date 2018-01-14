namespace UrbanSketchers.Data
{
    /// <summary>
    ///     Person interface
    /// </summary>
    public interface IPerson
    {
        /// <summary>
        ///     Gets or sets the Id of the person
        /// </summary>
        string Id { get; set; }

        /// <summary>
        ///     Gets or sets the User id of the person
        /// </summary>
        string UserId { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the user is an administrator
        /// </summary>
        bool IsAdministrator { get; set; }

        /// <summary>
        ///     Gets or sets the name of the person
        /// </summary>
        string Name { get; set; }
    }
}