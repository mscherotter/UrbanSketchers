using UrbanSketchers.Data;
using UrbanSketchers.Interfaces;

namespace UrbanSketchers
{
    /// <summary>
    ///     The sketch manager
    /// </summary>
    public class SketchManager : ServiceManager<Sketch, Person, Rating>, ISketchManager
    {
        /// <summary>
        ///     Gets the singleton Sketch Manager
        /// </summary>
        // public static SketchManager DefaultManager { get; } = new SketchManager();
    }
}