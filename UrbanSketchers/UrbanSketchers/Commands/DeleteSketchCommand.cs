using System;
using System.Globalization;
using Autofac;
using UrbanSketchers.Core;
using UrbanSketchers.Interfaces;
using UrbanSketchers.Properties;
using Xamarin.Forms;

namespace UrbanSketchers.Commands
{
    /// <summary>
    /// delete sketch command
    /// </summary>
    public class DeleteSketchCommand : IDeleteSketchCommand
    {
        private Page _page;

        /// <summary>
        /// can execute changed event
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Gets or sets the page
        /// </summary>
        public Page Page
        {
            get => _page;
            set
            {
                _page = value;

                CanExecuteChanged?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Can the command execute
        /// </summary>
        /// <param name="parameter">a <see cref="ISketch"/> instance</param>
        /// <returns>true if the parameter is a sketch, the page is not null and the sketch has an Id.</returns>
        public bool CanExecute(object parameter)
        {
            return parameter is ISketch sketch 
                   && _page != null 
                   && !string.IsNullOrWhiteSpace(sketch.Id);
        }

        /// <summary>
        /// Delete the sketch
        /// </summary>
        /// <param name="parameter">a <see cref="ISketch"/> instance</param>
        public async void Execute(object parameter)
        {
            if (parameter is ISketch sketch)
            {
                var person = await Container.Current.Resolve<ISketchManager>().GetCurrentUserAsync();

                if (person == null)
                    if (await App.Authenticator.AuthenticateAsync())
                        person = await Container.Current.Resolve<ISketchManager>().GetCurrentUserAsync();

                if (person == null)
                    return;

                if (person.Id != sketch.CreatedBy && !person.IsAdministrator)
                {
                    await Page.DisplayAlert("Cannot Delete Sketch",
                        string.Format(
                            CultureInfo.CurrentCulture,
                            "Only {0} or an administrator can delete this sketch.", sketch.CreatedByName),
                        Resources.OK);

                    return;
                }

                var response = await Page.DisplayAlert(
                    Resources.DeleteSketch,
                    Resources.PressOKToDeleteSketch,
                    Resources.OK,
                    Resources.Cancel);

                if (!response) return;

                await Container.Current.Resolve<ISketchManager>().DeleteAsync(sketch);

                await Page.Navigation.PopAsync(true);
            }
        }
    }
}