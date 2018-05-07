using System;
using System.Windows.Input;
using Autofac;
using Autofac.Core;
using UrbanSketchers.Pages;
using Xamarin.Forms;

namespace UrbanSketchers.Commands
{
    /// <summary>
    /// Navigation command
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NavigationCommand<T> : ICommand where T:class
    {
        private bool _isBusy;

        /// <summary>
        /// Can execute changed event handler
        /// </summary>

        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Can the command exeucte
        /// </summary>
        /// <param name="parameter">the parameter is not used</param>
        /// <returns>true if the command is not busy</returns>
        public bool CanExecute(object parameter)
        {
            return !_isBusy;
        }

        /// <summary>
        /// Navigate to a page
        /// </summary>
        /// <param name="parameter">the parameter is not used</param>
        public async void Execute(object parameter)
        {
            if (App.NavigationPage.CurrentPage is T) return;

            _isBusy = true;

            CanExecuteChanged?.Invoke(this, new EventArgs());

            var page = UrbanSketchers.Core.Container.Current.Resolve<T>();

            await App.NavigationPage.PushAsync(page as Page, true);

            if (Application.Current.MainPage is RootPage rootPage)
                rootPage.IsPresented = false;

            _isBusy = false;

            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}