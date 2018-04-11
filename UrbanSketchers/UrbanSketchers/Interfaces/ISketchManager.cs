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

        Task DeleteCurrentUserAsync();

        Task DeleteAsync(ISketch sketch);

        Task<ISketch> GetSketchAsync(string id);
    }
}
