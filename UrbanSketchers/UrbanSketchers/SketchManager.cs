/*
 * To add Offline Sync Support:
 *  1) Add the NuGet package Microsoft.Azure.Mobile.Client.SQLiteStore (and dependencies) to all client projects
 *  2) Uncomment the #define OFFLINE_SYNC_ENABLED
 *
 * For more information, see: http://go.microsoft.com/fwlink/?LinkId=620342
 */
//#define OFFLINE_SYNC_ENABLED

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using UrbanSketchers.Data;

#if OFFLINE_SYNC_ENABLED
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
#endif

namespace UrbanSketchers
{
    public class SketchManager
    {
#if OFFLINE_SYNC_ENABLED
        IMobileServiceSyncTable<Sketch> sketchTable;
#else
        private readonly IMobileServiceTable<Sketch> _sketchTable;
        private readonly IMobileServiceTable<Person> _peopleTable;
#endif

        private const string offlineDbPath = @"localstore.db";

        private SketchManager()
        {
            CurrentClient = new MobileServiceClient(Constants.ApplicationURL);

#if OFFLINE_SYNC_ENABLED
            var store = new MobileServiceSQLiteStore(offlineDbPath);
            store.DefineTable<TodoItem>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            this.client.SyncContext.InitializeAsync(store);

            this.todoTable = client.GetSyncTable<Sketch>();
#else
            _sketchTable = CurrentClient.GetTable<Sketch>();
            _peopleTable = CurrentClient.GetTable<Person>();
#endif
        }

        public static SketchManager DefaultManager { get; } = new SketchManager();

        public MobileServiceClient CurrentClient { get; }

        public bool IsOfflineEnabled => _sketchTable is IMobileServiceSyncTable<Sketch>;

        public async Task<ObservableCollection<Person>> GetPeopleAsync()
        {
            var items = await _peopleTable.ToEnumerableAsync();

            return new ObservableCollection<Person>(items);
        }

        public async Task<ObservableCollection<Sketch>> GetSketchsAsync(string personId, bool syncItems = false)
        {
            try
            {
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif

                var query = from item in _sketchTable
                    where item.CreatedBy == personId
                    orderby item.CreationDate descending 
                    select item;

                var sketches = await query.ToEnumerableAsync();

                return new ObservableCollection<Sketch>(sketches);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"Sync error: {0}", e.Message);
            }
            return null;
        }

        public async Task<ObservableCollection<Sketch>> GetSketchsAsync(bool syncItems = false)
        {
            try
            {
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
                var items = await _sketchTable.ToEnumerableAsync();

                return new ObservableCollection<Sketch>(items);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"Sync error: {0}", e.Message);
            }
            return null;
        }

        public async Task SaveAsync(Sketch item)
        {
            if (item.Id == null)
                await _sketchTable.InsertAsync(item);
            else
                await _sketchTable.UpdateAsync(item);
        }

        public async Task SaveAsync(Person item)
        {
            if (item.Id == null)
                await _peopleTable.InsertAsync(item);
            else
                await _peopleTable.UpdateAsync(item);
        }


#if OFFLINE_SYNC_ENABLED
        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await this.client.SyncContext.PushAsync();

                await this.todoTable.PullAsync(
                    //The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
                    //Use a different query name for each unique query in your program
                    "allTodoItems",
                    this.todoTable.CreateQuery());
            }
            catch (MobileServicePushFailedException exc)
            {
                if (exc.PushResult != null)
                {
                    syncErrors = exc.PushResult.Errors;
                }
            }

            // Simple error/conflict handling. A real application would handle the various errors like network conditions,
            // server conflicts and others via the IMobileServiceSyncHandler.
            if (syncErrors != null)
            {
                foreach (var error in syncErrors)
                {
                    if (error.OperationKind == MobileServiceTableOperationKind.Update && error.Result != null)
                    {
                        //Update failed, reverting to server's copy.
                        await error.CancelAndUpdateItemAsync(error.Result);
                    }
                    else
                    {
                        // Discard local change.
                        await error.CancelAndDiscardItemAsync();
                    }

                    Debug.WriteLine(@"Error executing sync operation. Item: {0} ({1}). Operation discarded.", error.TableName, error.Item["id"]);
                }
            }
        }
#endif
    }
}