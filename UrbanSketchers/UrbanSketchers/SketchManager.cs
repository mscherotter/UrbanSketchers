/*
 * To add Offline Sync Support:
 *  1) Add the NuGet package Microsoft.Azure.Mobile.Client.SQLiteStore (and dependencies) to all client projects
 *  2) Uncomment the #define OFFLINE_SYNC_ENABLED
 *
 * For more information, see: http://go.microsoft.com/fwlink/?LinkId=620342
 */
//#define OFFLINE_SYNC_ENABLED

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using UrbanSketchers.Data;
using UrbanSketchers.Support;

#if OFFLINE_SYNC_ENABLED
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
#endif

namespace UrbanSketchers
{
    /// <summary>
    ///     The sketch manager
    /// </summary>
    public class SketchManager
    {
#if OFFLINE_SYNC_ENABLED
        IMobileServiceSyncTable<Sketch> sketchTable;
#else
        private readonly IMobileServiceTable<Sketch> _sketchTable;
        private readonly IMobileServiceTable<Person> _peopleTable;
        private readonly IMobileServiceTable<Rating> _ratingTable;

        internal async Task<IEnumerable<Rating>> GetRatingsAsync(string sketchId)
        {
            var query = from item in _ratingTable
                where item.SketchId == sketchId
                orderby item.UpdatedAt descending
                select item;

            return await query.ToEnumerableAsync();
        }
#endif

        // ReSharper disable once UnusedMember.Local
        private const string OfflineDbPath = @"localstore.db";

        private SketchManager()
        {
            CurrentClient = new MobileServiceClient(Constants.ApplicationUrl);

#if OFFLINE_SYNC_ENABLED
            var store = new MobileServiceSQLiteStore(offlineDbPath);
            store.DefineTable<TodoItem>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            this.client.SyncContext.InitializeAsync(store);

            this.todoTable = client.GetSyncTable<Sketch>();
#else
            _sketchTable = CurrentClient.GetTable<Sketch>();
            _peopleTable = CurrentClient.GetTable<Person>();
            _ratingTable = CurrentClient.GetTable<Rating>();
#endif
        }

        /// <summary>
        /// search for a sketch with the text in the title, address, or description
        /// </summary>
        /// <param name="text">the text</param>
        /// <returns>an async task with a collection of sketches</returns>
        public Task<MobileServiceCollection<Sketch, Sketch>> SearchAsync(string text)
        {
            text = text.ToLower();

            var query = from item in _sketchTable
                where item.Title.ToLower().Contains(text) || 
                item.Description.ToLower().Contains(text) ||
                item.Address.ToLower().Contains(text)
                select item;

            return query.IncludeTotalCount().ToCollectionAsync();
        }

        /// <summary>
        ///     Delete the current user
        /// </summary>
        /// <returns>an async task</returns>
        public async Task DeleteCurrentUserAsync()
        {
            var user = await GetCurrentUserAsync();

            if (user == null)
                return;

            await _peopleTable.DeleteAsync(user);
        }

        /// <summary>
        ///     Gets the rating for a sketch
        /// </summary>
        /// <param name="sketchId">the sketch Id</param>
        /// <returns>an async task with the rating</returns>
        public async Task<Rating> GetRatingAsync(string sketchId)
        {
            var user = await GetCurrentUserAsync();

            if (user == null) return null;

            var query = from item in _ratingTable
                where item.PersonId == user.Id &&
                      item.SketchId == sketchId
                select item;

            var rating = await query.ToEnumerableAsync();

            return rating.FirstOrDefault();
        }

        internal async Task<Person> GetCurrentUserAsync()
        {
            if (CurrentClient.CurrentUser == null)
                return null;

            var query = from item in _peopleTable
                where item.UserId == CurrentClient.CurrentUser.UserId
                select item;

            var users = await query.ToEnumerableAsync();

            var currentUser = users.FirstOrDefault();

            //todo: this is My user Id for Microsoft ID
            if (currentUser != null && currentUser.UserId == "sid:f5ffa4adba1c703f583186000f8d71ef")
            {
                currentUser.IsAdministrator = true;
            }

            return currentUser;
        }

        /// <summary>Upload a file</summary>
        /// <param name="fileName"></param>
        /// <param name="stream"></param>
        /// <returns>an async task with the Uri of the file uploaded.</returns>
        /// <remarks>
        ///     see <![CDATA[https://code.msdn.microsoft.com/windowsapps/Upload-File-to-Windows-c9169190]]> and <![CDATA[https://docs.microsoft.com/en-us/azure/storage/files/storage-dotnet-how-to-use-files]]></remarks>
        public async Task<string> UploadAsync(string fileName, Stream stream)
        {
            var sketchFile = new SketchFile
            {
                BlobName = fileName,
                Container = "sketches",
                Permissions = "rw"
            };

            var response =
                await CurrentClient.InvokeApiAsync<SketchFile, GetSasTokenResponse>("GetUploadToken", sketchFile);

            var sasUri = new Uri(response.Uri);

            var sasToken = sasUri.Query.Substring(1);

            var credentials = new StorageCredentials(sasToken);

            var uriString = string.Format(
                CultureInfo.InvariantCulture,
                "https://{0}/{1}",
                sasUri.Host,
                sketchFile.Container);

            var container = new CloudBlobContainer(new Uri(uriString), credentials);

            var blob = container.GetBlockBlobReference(fileName);

            try
            {
                await blob.UploadFromStreamAsync(stream);
            }
            catch (StorageException se)
            {
                Debug.WriteLine(
                    $"Error uploading {fileName}: {se.RequestInformation.ExtendedErrorInformation.ErrorMessage}.");

                return null;
            }

            var uri = response.Uri.Substring(0, response.Uri.IndexOf('?'));

            return uri;
        }

        internal Task DeleteAsync(ISketch sketch)
        {
            return _sketchTable.DeleteAsync(sketch as Sketch);
        }

        /// <summary>
        ///     Gets the singleton Sketch Manager
        /// </summary>
        public static SketchManager DefaultManager { get; } = new SketchManager();

        /// <summary>
        ///     Gets the current Mobile service client
        /// </summary>
        public MobileServiceClient CurrentClient { get; }

        /// <summary>
        ///     Gets or sets the thumbnail generator
        /// </summary>
        public IThumbnailGenerator ThumbnailGenerator { get; set; }

        /// <summary>
        ///     Gets a value indicating whether offline support is enabled
        /// </summary>
        // ReSharper disable once SuspiciousTypeConversion.Global
        public bool IsOfflineEnabled => _sketchTable is IMobileServiceSyncTable<Sketch>;

        /// <summary>
        ///     Get a sketch
        /// </summary>
        /// <param name="id">the sketch Id</param>
        /// <returns>an async task with the sketch</returns>
        public async Task<ISketch> GetSketchAsync(string id)
        {
            var sketch = await _sketchTable.LookupAsync(id);

            return sketch;
        }

        /// <summary>
        ///     Gets the people who are sketchers
        /// </summary>
        /// <returns>an async task with an observable collection of people</returns>
        public async Task<ObservableCollection<Person>> GetPeopleAsync()
        {
            var query = from item in _peopleTable
                orderby item.Name
                select item;

            var items = await query.ToEnumerableAsync();

            return new ObservableCollection<Person>(items);
        }

        /// <summary>
        ///     Gets the sketches
        /// </summary>
        /// <param name="personId">the person who created the sketches</param>
        /// <param name="syncItems">true to sync the offline items</param>
        /// <returns>an async task with an observable collection of sketches</returns>
        public async Task<ObservableCollection<ISketch>> GetSketchsAsync(string personId, bool syncItems = false)
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

                return new ObservableCollection<ISketch>(sketches);
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

        /// <summary>
        ///     Get the sketches in a specific sector
        /// </summary>
        /// <param name="sector">the sector</param>
        /// <returns>an async task with an observable collection of sketches</returns>
        public Task<MobileServiceCollection<Sketch, Sketch>> GetSketchsAsync(int sector)
        {
            var items = from item in _sketchTable
                where item.Sector == sector
                select item;

            return items.IncludeTotalCount().ToCollectionAsync();
        }

        /// <summary>
        ///     Gets the sketches
        /// </summary>
        /// <returns>an async task with a collection of sketches</returns>
        public async Task<MobileServiceCollection<Sketch, Sketch>> GetSketchsAsync()
        {
            try
            {
                var collection = await _sketchTable.IncludeTotalCount().ToCollectionAsync();

                return collection;
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"Sync error: {0}", e.Message);
            }

            return null;
        }

        /// <summary>
        ///     Insert or update the sketch
        /// </summary>
        /// <param name="item">the sketch</param>
        /// <returns>an async task</returns>
        public async Task SaveAsync(ISketch item)
        {
            item.Sector = CustomIndexing.LatLonToSector(item.Latitude, item.Longitude, CustomIndexing.SectorSize);

            if (string.IsNullOrWhiteSpace(item.Id))
                await _sketchTable.InsertAsync(item as Sketch);
            else
                await _sketchTable.UpdateAsync(item as Sketch);
        }

        /// <summary>
        ///     Insert or update a Person
        /// </summary>
        /// <param name="item">a person</param>
        /// <returns>an async task</returns>
        public async Task SaveAsync(Person item)
        {
            if (item.Id == null)
                await _peopleTable.InsertAsync(item);
            else
                await _peopleTable.UpdateAsync(item);
        }

        /// <summary>
        ///     Insert or update a rating
        /// </summary>
        /// <param name="item">the rating</param>
        /// <returns>an async task</returns>
        public async Task SaveAsync(Rating item)
        {
            if (item.Id == null)
                await _ratingTable.InsertAsync(item);
            else
                await _ratingTable.UpdateAsync(item);
        }

        internal Task<Person> GetPersonAsync(string personId)
        {
            return _peopleTable.LookupAsync(personId);
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