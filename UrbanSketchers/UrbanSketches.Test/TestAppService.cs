
using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace UrbanSketches.Test
{
    [TestClass]
    public class TestAppService
    {
        [TestMethod]
        public async Task TestUploadAsync()
        {
            using (var connection = new AppServiceConnection
            {
                AppServiceName = "Upload.1",
                PackageFamilyName = "MichaelS.Scherotter.UrbanSketches_9eg5g21zq32qm"
            })
            {
                var openStatus = await connection.OpenAsync();

                Assert.AreEqual(AppServiceConnectionStatus.Success, openStatus);

                if (openStatus != AppServiceConnectionStatus.Success)
                {
                    return;
                }

                var file =
                    await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Page 50-51.jpg"));

                var message = new ValueSet
                {
                    ["Method"]    = "Upload",
                    ["FileToken"] = SharedStorageAccessManager.AddFile(file)
                };

                var response = await connection.SendMessageAsync(message);

                Assert.AreEqual(AppServiceResponseStatus.Success, response);

                Assert.IsTrue((bool) response.Message["Succeeded"]);
            }


        }

        [TestMethod]
        public async Task TestUploadNoMethodAsync()
        {
            using (var connection = new AppServiceConnection
            {
                AppServiceName = "Upload.1",
                PackageFamilyName = "MichaelS.Scherotter.UrbanSketches_9eg5g21zq32qm"
            })
            {
                var openStatus = await connection.OpenAsync();

                Assert.AreEqual(AppServiceConnectionStatus.Success, openStatus);

                if (openStatus != AppServiceConnectionStatus.Success)
                {
                    return;
                }

                var message = new ValueSet
                {
                };

                var response = await connection.SendMessageAsync(message);

                Assert.AreEqual(AppServiceResponseStatus.Success, response);

                Assert.IsFalse((bool)response.Message["Succeeded"]);
            }


        }

        [TestMethod]
        public async Task TestUploadNoFileTokenAsync()
        {
            using (var connection = new AppServiceConnection
            {
                AppServiceName = "Upload.1",
                PackageFamilyName = "MichaelS.Scherotter.UrbanSketches_9eg5g21zq32qm"
            })
            {
                var openStatus = await connection.OpenAsync();

                Assert.AreEqual(AppServiceConnectionStatus.Success, openStatus);

                if (openStatus != AppServiceConnectionStatus.Success)
                {
                    return;
                }

                var message = new ValueSet
                {
                    ["Method"] = "Upload"
                };

                var response = await connection.SendMessageAsync(message);

                Assert.AreEqual(AppServiceResponseStatus.Success, response);

                Assert.IsFalse((bool)response.Message["Succeeded"]);
            }


        }

        [TestMethod]
        public async Task TestUploadInvalidFileTokenAsync()
        {
            using (var connection = new AppServiceConnection
            {
                AppServiceName = "Upload.1",
                PackageFamilyName = "MichaelS.Scherotter.UrbanSketches_9eg5g21zq32qm"
            })
            {
                var openStatus = await connection.OpenAsync();

                Assert.AreEqual(AppServiceConnectionStatus.Success, openStatus);

                if (openStatus != AppServiceConnectionStatus.Success)
                {
                    return;
                }

                var message = new ValueSet
                {
                    ["Method"] = "Upload",
                    ["FileToken"] = Guid.NewGuid().ToString()
                };

                var response = await connection.SendMessageAsync(message);

                Assert.AreEqual(AppServiceResponseStatus.Success, response);

                Assert.IsFalse((bool)response.Message["Succeeded"]);
            }    
        }
    }
}
