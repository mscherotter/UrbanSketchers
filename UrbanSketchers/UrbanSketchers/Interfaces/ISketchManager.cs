using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using UrbanSketchers.Data;

namespace UrbanSketchers.Interfaces
{
    public interface ISketchManager
    {
        Task<IEnumerable<IPerson>> GetPeopleAsync();

        IThumbnailGenerator ThumbnailGenerator { get; set; }

        MobileServiceClient CurrentClient { get; }

        Task<string> UploadAsync(string fileName, Stream stream);

        Task<IEnumerable<ISketch>> SearchAsync(string text);

        Task<IEnumerable<ISketch>> GetSketchsAsync(string personId, bool syncItems = false);

        Task SaveAsync(IRating item);

        Task SaveAsync(ISketch item);
        Task SaveAsync(IPerson item);

        Task<IEnumerable<ISketch>> GetSketchsAsync();

        Task<IEnumerable<ISketch>> GetSketchsAsync(int sector);

        Task<IEnumerable<IRating>> GetRatingsAsync(string sketchId);

        Task<IRating> GetRatingAsync(string sketchId);

        Task<IPerson> GetPersonAsync(string personId);

        Task<IPerson> GetCurrentUserAsync();

        Task DeleteAsync(ISketch sketch);

        /// <summary>
        /// Delete a person
        /// </summary>
        /// <param name="person">the person</param>
        /// <returns>an async task</returns>
        Task DeleteAsync(IPerson person);

        /// <summary>
        /// Gets a sketch
        /// </summary>
        /// <param name="id">the sketch Id</param>
        /// <returns>an aysnc task with a sketch</returns>
        Task<ISketch> GetSketchAsync(string id);

        /// <summary>
        /// Gets the user data HTML for a person
        /// </summary>
        /// <param name="personId">the user id</param>
        /// <returns>an async task with the user</returns>
        Task<string> GetUserDataAsync(string personId);
    }
}
