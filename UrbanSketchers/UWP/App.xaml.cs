using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Autofac;
using Microsoft.WindowsAzure.MobileServices;
using UrbanSketchers.Core;
using UrbanSketchers.Interfaces;
using UrbanSketchers.Support;
using UWP.Support;
using Xamarin.Forms;
using Frame = Windows.UI.Xaml.Controls.Frame;

namespace UWP
{
    /// <summary>
    ///     Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App
    {
        /// <summary>
        ///     Initializes the singleton application object.  This is the first line of authored code
        ///     executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();

            Suspending += OnSuspending;

            FilePickerService.Current = new UWPFilePickerService();
        }

        /// <summary>
        ///     Invoked when the application is launched normally by the end user.  Other entry points
        ///     will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (Debugger.IsAttached)
                DebugSettings.EnableFrameRateCounter = true;
#endif

            var rootFrame = CreateRootFrame(e);

            if (rootFrame.Content == null)
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        ///     Background activated
        /// </summary>
        /// <param name="args">the background activated event arguments</param>
        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            if (args.TaskInstance.TriggerDetails is AppServiceTriggerDetails details)
                details.AppServiceConnection.RequestReceived += AppServiceConnection_RequestReceived;
        }

        private async void AppServiceConnection_RequestReceived(AppServiceConnection sender,
            AppServiceRequestReceivedEventArgs args)
        {
            var deferral = args.GetDeferral();

            var request = args.Request;

            var message = request.Message;

            if (message.TryGetValue("Method", out var value))
            {
                var method = value.ToString();

                if (method.Equals("upload", StringComparison.OrdinalIgnoreCase))
                    await UploadAsync(request, message);
                else
                    await SendFailureResponseAsync(request, "The only valid Method is 'Upload'");
            }
            else
            {
                await SendFailureResponseAsync(request, "The Message must contain a Method, FileToken and Title");
            }

            deferral.Complete();
        }

        private static async Task SendFailureResponseAsync(AppServiceRequest request, string message)
        {
            var response = new ValueSet
            {
                ["ErrorMessage"] = message,
                ["Succeeded"] = false
            };

            await request.SendResponseAsync(response);
        }

        ////async Task UploadAsync(StorageFile fileToUpload)
        ////{
        ////    using (var connection = new AppServiceConnection
        ////    {
        ////        AppServiceName = "Upload.1",
        ////        PackageFamilyName = "MichaelS.Scherotter.UrbanSketches_9eg5g21zq32qm"
        ////    })
        ////    {
        ////        var status = await connection.OpenAsync();

        ////        if (status == AppServiceConnectionStatus.Success)
        ////        {
        ////            var message = new ValueSet
        ////            {
        ////                ["Method"] = "Upload",
        ////                ["Title"] = "A sketch",
        ////                ["Address"] = "Optional address",
        ////                ["CreationDate"] = DateTime.UtcNow,
        ////                ["Description"] = "Optional description",
        ////                ["FileToken"] = SharedStorageAccessManager.AddFile(fileToUpload),
        ////                ["Latitude"] = 40.0,
        ////                ["Longitude"] = 100.3
        ////            };

        ////            var response = await connection.SendMessageAsync(message);

        ////            bool succeeded = (bool) response.Message["Success"];
        ////        }
        ////    }
        ////}

        /// <summary>
        ///     Upload
        /// </summary>
        /// <param name="request"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private static async Task UploadAsync(AppServiceRequest request, ValueSet message)
        {
            var sketch = Container.Current.Resolve<ISketch>();

            StorageFile file = null;

            if (message.TryGetValue("FileToken", out var fileToken))
                file = await SharedStorageAccessManager.RedeemTokenForFileAsync(fileToken.ToString());

            if (file == null)
            {
                await SendFailureResponseAsync(request, "The Message must contain a FileToken");
            }
            else
            {
                var imageProperties = await file.Properties.GetImagePropertiesAsync();

                if (message.TryGetValue("Title", out var title))
                    sketch.Title = title.ToString();
                else
                    sketch.Title = imageProperties.Title;

                if (string.IsNullOrWhiteSpace(sketch.Title))
                {
                    await SendFailureResponseAsync(request,
                        "Message must have a Title property or the image must have a Title EXIF property.");

                    return;
                }

                if (message.TryGetValue("CreationDate", out var dateCreated))
                    sketch.CreationDate = (DateTime) dateCreated;
                else
                    sketch.CreationDate = imageProperties.DateTaken.ToUniversalTime().DateTime;
                if (message.TryGetValue("Address", out var address))
                    sketch.Address = address.ToString();

                if (message.TryGetValue("Latitude", out var latitude))
                    sketch.Latitude = (double) latitude;
                else if (imageProperties.Latitude.HasValue)
                    sketch.Latitude = imageProperties.Latitude.Value;

                if (message.TryGetValue("Longitude", out var longitude))
                    sketch.Longitude = (double) longitude;
                else if (imageProperties.Longitude.HasValue)
                    sketch.Longitude = imageProperties.Longitude.Value;

                if (message.TryGetValue("Description", out var description))
                    sketch.Description = description.ToString();

                using (var stream = await file.OpenStreamForReadAsync())
                {
                    try
                    {
                        sketch.ImageUrl =
                            await Container.Current.Resolve<ISketchManager>().UploadAsync(file.Name, stream);

                        await Container.Current.Resolve<ISketchManager>().SaveAsync(sketch);
                    }
                    catch (Exception e)
                    {
                        await SendFailureResponseAsync(request, e.Message);

                        return;
                    }
                }

                var response = new ValueSet
                {
                    ["Succeeded"] = true
                };

                await request.SendResponseAsync(response);
            }
        }

        /// <summary>
        ///     Activated
        /// </summary>
        /// <param name="args">the activated event arguments</param>
        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);

            switch (args.Kind)
            {
                case ActivationKind.Protocol:
                    if (args is ProtocolActivatedEventArgs protocolArgs)
                        if (protocolArgs.Uri.ToString().StartsWith("urbansketchesauth:"))
                            Container.Current.Resolve<ISketchManager>().CurrentClient.ResumeWithURL(protocolArgs.Uri);
                    break;
            }
        }

        private Frame CreateRootFrame(LaunchActivatedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e != null)
                {
                    Forms.Init(e);

                    if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                    {
                        //TODO: Load state from previously suspended application
                    }
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            return rootFrame;
        }

        /// <summary>
        ///     Share target activated
        /// </summary>
        /// <param name="args">the share target activated event arguments</param>
        protected override void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
        {
            var frame = CreateRootFrame(null);

            frame.Navigate(typeof(ShareTargetPage), args.ShareOperation);

            Window.Current.Activate();
        }

        /// <summary>
        ///     Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        ///     Invoked when application execution is being suspended.  Application state is saved
        ///     without knowing whether the application will be terminated or resumed with the contents
        ///     of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}