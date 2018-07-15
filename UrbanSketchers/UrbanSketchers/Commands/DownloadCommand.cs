using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Plugin.FilePicker.Abstractions;
using UrbanSketchers.Interfaces;
using UrbanSketchers.Support;

namespace UrbanSketchers.Commands
{
    public class DownloadCommand : IDownloadCommand
    {
        private string _personId;
        private bool _canDownload;
        private bool _isBusy;

        public DownloadCommand(ISketchManager sketchManager)
        {
            SketchManager = sketchManager;
        }

        public string PersonId
        {
            get => _personId;
            set
            {
                _personId = value;

                UpdatePerson();
            }

        }

        public ISketchManager SketchManager { get; }

        async void UpdatePerson()
        {
            var person = await SketchManager.GetCurrentUserAsync();

            if (person == null) return;

            _canDownload = person.Id == PersonId || person.IsAdministrator;

            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canDownload && !_isBusy;
        }

        /// <summary>
        /// Save the user data as an HTML file
        /// </summary>
        /// <param name="parameter">the parameter is not used</param>
        public async void Execute(object parameter)
        {
            try
            {
                _isBusy = true;

                CanExecuteChanged?.Invoke(this, new EventArgs());

                var html = await SketchManager.GetUserDataAsync(PersonId);

                if (string.IsNullOrWhiteSpace(html)) return;

                using (var stream = new MemoryStream())
                {
                    var writer = new StreamWriter(stream);

                    await writer.WriteAsync(html);

                    stream.Seek(0, SeekOrigin.Begin);

                    var fileData = new FileData
                    {
                        DataArray = stream.GetBuffer(),
                        FileName = "My Urban Sketches Data.html"
                    };

                    await FilePickerService.Current.PickSaveFileAsync(fileData, FilePickerService.LocationId.Documents);
                }
            }
            finally
            {
                _isBusy = false;

                CanExecuteChanged?.Invoke(this, new EventArgs());
            }
        }
    }
}
