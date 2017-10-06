using System;
using System.Windows.Input;
using UrbanSketchers.Views;
using Xamarin.Forms;

namespace UrbanSketchers.Commands
{
    /// <summary>
    /// Navigation command
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NavigationCommand<T> : ICommand where T : new()
    {
        private bool _isBusy;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return !_isBusy;
        }

        public async void Execute(object parameter)
        {
            if (App.NavigationPage.CurrentPage is T) return;

            _isBusy = true;

            CanExecuteChanged?.Invoke(this, new EventArgs());

            await App.NavigationPage.PushAsync(new T() as Page, true);

            if (Application.Current.MainPage is RootPage rootPage)
                rootPage.IsPresented = false;

            _isBusy = false;

            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}