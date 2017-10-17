using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using UrbanSketchers;

namespace UWP
{
    public class ThumbnailGenerator : IThumbnailGenerator
    {
        public async Task<Stream> CreateThumbnailAsync(byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
            {
                var decoder = await BitmapDecoder.CreateAsync(memoryStream.AsRandomAccessStream());

                using (var imageStream = await decoder.GetThumbnailAsync())
                {
                    var thumbnailStream = new MemoryStream();

                    await imageStream.AsStreamForRead().CopyToAsync(thumbnailStream);

                    thumbnailStream.Seek(0, SeekOrigin.Begin);

                    return thumbnailStream;
                }
            }
        }
    }
}