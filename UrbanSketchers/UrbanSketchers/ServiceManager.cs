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
using UrbanSketchers.Interfaces;
using UrbanSketchers.Support;
using System.Net.Http;
#if OFFLINE_SYNC_ENABLED
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;

#endif


namespace UrbanSketchers
{
    /// <summary>
    ///     Service manager
    /// </summary>
    /// <typeparam name="TSketch">the sketch type</typeparam>
    /// <typeparam name="TPerson">the person type</typeparam>
    /// <typeparam name="TRating">the rating type</typeparam>
    public class ServiceManager<TSketch, TPerson, TRating>
        where TSketch : class, ISketch
        where TPerson : class, IPerson
        where TRating : class, IRating
    {
#if OFFLINE_SYNC_ENABLED
        private readonly IMobileServiceSyncTable<TSketch> _sketchTable;
        private readonly IMobileServiceSyncTable<TPerson> _peopleTable;
        private readonly IMobileServiceSyncTable<TRating> _ratingTable;
#else
        private readonly IMobileServiceTable<TSketch> _sketchTable;
        private readonly IMobileServiceTable<TPerson> _peopleTable;
        private readonly IMobileServiceTable<TRating> _ratingTable;
#endif

        public async Task<IEnumerable<IRating>> GetRatingsAsync(string sketchId)
        {
            var query = from item in _ratingTable
                where item.SketchId == sketchId
                orderby item.UpdatedAt descending
                select item;

            return await query.ToEnumerableAsync();
        }

        // ReSharper disable once UnusedMember.Local
        private const string OfflineDbPath = @"localstore.db";

        /// <summary>
        ///     Initializes a new instance of the ServiceManager class
        /// </summary>
        protected ServiceManager()
        {
            CurrentClient = new MobileServiceClient(Constants.ApplicationUrl);

#if OFFLINE_SYNC_ENABLED
            var store = new MobileServiceSQLiteStore(OfflineDbPath);
            store.DefineTable<TSketch>();
            store.DefineTable<TPerson>();
            store.DefineTable<TRating>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            CurrentClient.SyncContext.InitializeAsync(store);

            _sketchTable = CurrentClient.GetSyncTable<TSketch>();
            _peopleTable = CurrentClient.GetSyncTable<TPerson>();
            _ratingTable = CurrentClient.GetSyncTable<TRating>();
#else
            _sketchTable = CurrentClient.GetTable<TSketch>();
            _peopleTable = CurrentClient.GetTable<TPerson>();
            _ratingTable = CurrentClient.GetTable<TRating>();
#endif
        }

        /// <summary>
        ///     search for a sketch with the text in the title, address, or description
        /// </summary>
        /// <param name="text">the text</param>
        /// <returns>an async task with a collection of sketches</returns>
        public async Task<IEnumerable<ISketch>> SearchAsync(string text)
        {
            text = text.ToLower();

            var query = from item in _sketchTable
                where item.Title.ToLower().Contains(text) ||
                      item.Description.ToLower().Contains(text) ||
                      item.Address.ToLower().Contains(text)
                select item;

            var collection = await query.IncludeTotalCount().ToCollectionAsync();

            return collection;
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

            await _peopleTable.DeleteAsync(user as TPerson);
        }

        /// <summary>
        ///     Gets the rating for a sketch
        /// </summary>
        /// <param name="sketchId">the sketch Id</param>
        /// <returns>an async task with the rating</returns>
        public async Task<IRating> GetRatingAsync(string sketchId)
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

        public async Task<IPerson> GetCurrentUserAsync()
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
                currentUser.IsAdministrator = true;

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
            try
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

                await blob.UploadFromStreamAsync(stream);

                var uri = response.Uri.Substring(0, response.Uri.IndexOf('?'));

                return uri;
            }
            catch (StorageException se)
            {
                Debug.WriteLine(
                    $"Error uploading {fileName}: {se.RequestInformation.ExtendedErrorInformation.ErrorMessage}.");
            }
            catch (HttpRequestException hre)
            {
                Debug.WriteLine($"Failed to upload file {hre.Message}. Caching filename in local storage...");

                var file = await PCLStorage.FileSystem.Current.LocalStorage.CreateFileAsync(fileName, PCLStorage.CreationCollisionOption.GenerateUniqueName);

                using (var fileStream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
                {
                    await stream.CopyToAsync(fileStream);
                }

                return fileName;
            }

            return null;
        }

        public Task DeleteAsync(ISketch sketch)
        {
            return _sketchTable.DeleteAsync(sketch as TSketch);
        }

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
        // ReSharper disable once IsExpressionAlwaysTrue
        public bool IsOfflineEnabled => _sketchTable is IMobileServiceSyncTable<TSketch>;

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
        public async Task<IEnumerable<IPerson>> GetPeopleAsync()
        {
            var query = from item in _peopleTable
                orderby item.Name
                select item;

            var items = await query.ToEnumerableAsync();

            return new ObservableCollection<TPerson>(items);
        }

        /// <summary>
        ///     Gets the sketches
        /// </summary>
        /// <param name="personId">the person who created the sketches</param>
        /// <param name="syncItems">true to sync the offline items</param>
        /// <returns>an async task with an observable collection of sketches</returns>
        public async Task<IEnumerable<ISketch>> GetSketchsAsync(string personId, bool syncItems = false)
        {
            try
            {
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                    await SyncAsync();
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
        public async Task<IEnumerable<ISketch>> GetSketchsAsync(int sector)
        {
            var items = from item in _sketchTable
                where item.Sector == sector
                select item;

            var sketches = await items.IncludeTotalCount().ToCollectionAsync();

            return sketches;
        }

        /// <summary>
        ///     Gets the sketches
        /// </summary>
        /// <returns>an async task with a collection of sketches</returns>
        public async Task<IEnumerable<ISketch>> GetSketchsAsync()
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
            {
                item.CreatedByName = "You";
                await _sketchTable.InsertAsync(item as TSketch);
            }
            else
                await _sketchTable.UpdateAsync(item as TSketch);
        }

        /// <summary>
        ///     Insert or update a Person
        /// </summary>
        /// <param name="item">a person</param>
        /// <returns>an async task</returns>
        public async Task SaveAsync(IPerson item)
        {
            if (item.Id == null)
                await _peopleTable.InsertAsync(item as TPerson);
            else
                await _peopleTable.UpdateAsync(item as TPerson);
        }

        /// <summary>
        ///     Insert or update a rating
        /// </summary>
        /// <param name="item">the rating</param>
        /// <returns>an async task</returns>
        public async Task SaveAsync(IRating item)
        {
            if (item.Id == null)
                await _ratingTable.InsertAsync(item as TRating);
            else
                await _ratingTable.UpdateAsync(item as TRating);
        }

        public async Task<IPerson> GetPersonAsync(string personId)
        {
            return await _peopleTable.LookupAsync(personId);
        }


#if OFFLINE_SYNC_ENABLED
        /// <summary>
        /// Sync the offline table
        /// </summary>
        /// <returns>an async task</returns>
        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await CurrentClient.SyncContext.PushAsync();

                await _sketchTable.PullAsync(
                    //The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
                    //Use a different query name for each unique query in your program
                    "allSketches",
                    _sketchTable.CreateQuery());

                await _peopleTable.PullAsync(
                    //The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
                    //Use a different query name for each unique query in your program
                    "allPeople",
                    _peopleTable.CreateQuery());

                await _ratingTable.PullAsync(
                    //The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
                    //Use a different query name for each unique query in your program
                    "allRatings",
                    _ratingTable.CreateQuery());
            }
            catch (MobileServicePushFailedException exc)
            {
                if (exc.PushResult != null)
                    syncErrors = exc.PushResult.Errors;
            }

            // Simple error/conflict handling. A real application would handle the various errors like network conditions,
            // server conflicts and others via the IMobileServiceSyncHandler.
            if (syncErrors != null)
                foreach (var error in syncErrors)
                {
                    if (error.OperationKind == MobileServiceTableOperationKind.Update && error.Result != null)
                        await error.CancelAndUpdateItemAsync(error.Result);
                    else
                        await error.CancelAndDiscardItemAsync();

                    Debug.WriteLine(@"Error executing sync operation. Item: {0} ({1}). Operation discarded.",
                        error.TableName, error.Item["id"]);
                }
        }
#endif
    }
}