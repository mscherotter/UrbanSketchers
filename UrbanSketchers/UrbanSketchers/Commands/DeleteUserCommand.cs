using System;
using System.Collections.Generic;
using System.Text;
using UrbanSketchers.Interfaces;
using Xamarin.Forms;

namespace UrbanSketchers.Commands
{
    public class DeleteUserCommand : IDeleteUserCommand
    {
        private string _personId;
        private bool _canDelete;
        private bool _isBusy;
        private IPerson _person;

        public DeleteUserCommand(ISketchManager sketchManager)
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
        public event EventHandler CanExecuteChanged;

        public ISketchManager SketchManager { get; }

        async void UpdatePerson()
        {
            _person = await SketchManager.GetCurrentUserAsync();

            if (_person == null)
            {
                _canDelete = false;
            }
            else
            {
                _canDelete = _person.Id == PersonId || _person.IsAdministrator;
            }

            CanExecuteChanged?.Invoke(this, new EventArgs());
        }


        public bool CanExecute(object parameter)
        {
            return _canDelete && !_isBusy && parameter is Page;
        }

        public async void Execute(object parameter)
        {
            try
            {
                var page = parameter as Page;
                _isBusy = true;

                CanExecuteChanged?.Invoke(this, new EventArgs());

                if (!await page.DisplayAlert(Properties.Resources.RemoveUser,
                    Properties.Resources.RemoveUserQuestion,
                    Properties.Resources.OK, Properties.Resources.Cancel))
                    return;

                await SketchManager.DeleteAsync(_person);
            }
            finally
            {
                _isBusy = false;

                CanExecuteChanged?.Invoke(this, new EventArgs());
            }
        }
    }
}
