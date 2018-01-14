/*
 * To add Offline Sync Support:
 *  1) Add the NuGet package Microsoft.Azure.Mobile.Client.SQLiteStore (and dependencies) to all client projects
 *  2) Uncomment the #define OFFLINE_SYNC_ENABLED
 *
 * For more information, see: http://go.microsoft.com/fwlink/?LinkId=620342
 */
//#define OFFLINE_SYNC_ENABLED

using UrbanSketchers.Data;

#if OFFLINE_SYNC_ENABLED
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
#endif

namespace UrbanSketchers
{
    /// <summary>
    ///     The sketch manager
    /// </summary>
    public class SketchManager : ServiceManager<Sketch, Person, Rating>
    {
        /// <summary>
        ///     Gets the singleton Sketch Manager
        /// </summary>
        public static SketchManager DefaultManager { get; } = new SketchManager();
    }
}