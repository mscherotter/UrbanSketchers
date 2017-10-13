# Urban Sketchers
Work in Progress by Michael S. Scherotter

Urban Sketchers Mobile App will map urban sketches from around the world.

## Sketch
Each *Sketch* has the following properties
* Title (String)
* Description (Optional String)
* Address (Optional String)
* Latitude (double)
* Longitude (double)
* Image Url (String)
* Creation Date (Date/time)
* Created By (Person)
* Thumbnail Url (String)
* Sector (integer)

## Person
Each *Person* has the following properties
* Name (String)
* Image Url (String)
* Public Url (String)

## Rating
Each *Rating* has the following properties
* Sketch (String)
* Person (String)
* Like (Boolean)
* Comment (String)
* Is Violation (Boolean)

# Upload App Service
Package Family Name: MichaelS.Scherotter.UrbanSketches_9eg5g21zq32qm
App Service Name: Upload.1
## Message Parameters:
Method: Required "Upload"
Title: Required string
Address: Optional string
Description: Optional string
CreationDate: Optional DateTime
FileToken: Required, A token created with SharedStorageAccessManager.AddFile()
## Sample Code
    async Task UploadAsync(StorageFile fileToUpload)
    {
        using (var connection = new AppServiceConnection
        {
            AppServiceName = "Upload.1",
            PackageFamilyName = "MichaelS.Scherotter.UrbanSketches_9eg5g21zq32qm"
        })
        {
            var status = await connection.OpenAsync();

            if (status == AppServiceConnectionStatus.Success)
            {
                var message = new ValueSet
                {
                    ["Method"] = "Upload",
                    ["Title"] = "A sketch",
                    ["Address"] = "Optional address",
                    ["CreationDate"] = DateTime.UtcNow,
                    ["Description"] = "Optional description",
                    ["FileToken"] = SharedStorageAccessManager.AddFile(fileToUpload),
                    ["Latitude"] = 40.0,
                    ["Longitude"] = 100.3
                };

                var response = await connection.SendMessageAsync(message);

                bool succeeded = (bool) response.Message["Success"];
            }
        }
    }
