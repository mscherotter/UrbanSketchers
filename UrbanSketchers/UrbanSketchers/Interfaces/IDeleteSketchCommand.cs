using System.Windows.Input;
using Xamarin.Forms;

namespace UrbanSketchers.Interfaces
{
    /// <summary>
    ///     Delete sketch command interface
    /// </summary>
    public interface IDeleteSketchCommand : ICommand
    {
        /// <summary>
        ///     gets or sets the page
        /// </summary>
        Page Page { get; set; }
    }
}