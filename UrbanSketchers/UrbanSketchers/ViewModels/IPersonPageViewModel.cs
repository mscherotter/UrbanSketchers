using System.Threading.Tasks;

namespace UrbanSketchers.ViewModels
{
    /// <summary>
    ///     Person page view model interface
    /// </summary>
    public interface IPersonPageViewModel
    {
        /// <summary>
        ///     Gets or sets the person Id
        /// </summary>
        string PersonId { get; set; }

        /// <summary>
        ///     Refresh the person and sketches for that person
        /// </summary>
        /// <returns></returns>
        Task RefreshAsync();
    }
}