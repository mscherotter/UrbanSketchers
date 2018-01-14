using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UrbanSketchers.Commands;
using UrbanSketchers.Data;
using UrbanSketchers.Helpers;

namespace UrbanSketchers.ViewModels
{
    /// <summary>
    ///     Person page view model
    /// </summary>
    public class PersonPageViewModel : ObservableObject, IPersonPageViewModel
    {
        private bool _isBusy;
        private Person _person;
        private RelayCommand<object> _refreshCommand;

        /// <summary>
        ///     Initializes a new instance of the PersonPageViewModel class.
        /// </summary>
        public PersonPageViewModel()
        {
            RefreshCommand = new RelayCommand<object>(OnRefresh, CanRefresh);
            UpdateCommand = new RelayCommand<object>(OnUpdate, CanUpdate);
            DeleteCommand = new RelayCommand<object>(OnDelete, CanDelete);

            Sketches = new ObservableCollection<Sketch>();
        }

        /// <summary>
        ///     Gets or sets the person
        /// </summary>
        public Person Person
        {
            get => _person;
            set => SetProperty(ref _person, value);
        }

        /// <summary>
        ///     Gets or sets the refresh command
        /// </summary>
        public RelayCommand<object> RefreshCommand
        {
            get => _refreshCommand;
            set => SetProperty(ref _refreshCommand, value);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the view model is busy
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (SetProperty(ref _isBusy, value))
                {
                    RefreshCommand.RaiseCanExecuteChanged();
                    UpdateCommand.RaiseCanExecuteChanged();
                    DeleteCommand.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        ///     Gets the update command
        /// </summary>
        public RelayCommand<object> UpdateCommand { get; set; }

        /// <summary>
        ///     Gets the delete command
        /// </summary>
        public RelayCommand<object> DeleteCommand { get; set; }


        /// <summary>
        ///     Gets the sketches
        /// </summary>
        public ObservableCollection<Sketch> Sketches { get; }

        /// <summary>
        ///     Gets or sets the person Id
        /// </summary>
        public string PersonId { get; set; }

        /// <summary>
        ///     Refresh the person and sketches
        /// </summary>
        /// <returns></returns>
        public async Task RefreshAsync()
        {
            Person = await SketchManager.DefaultManager.GetPersonAsync(PersonId);

            var sketches = await SketchManager.DefaultManager.GetSketchsAsync(PersonId);

            Sketches.SetRange(sketches);
        }

        private bool CanRefresh(object arg)
        {
            return !IsBusy;
        }

        private async void OnRefresh(object obj)
        {
            IsBusy = true;

            await RefreshAsync();

            IsBusy = false;
        }

        private bool CanUpdate(object arg)
        {
            return !IsBusy;
        }

        private async void OnUpdate(object obj)
        {
            IsBusy = true;

            await SketchManager.DefaultManager.SaveAsync(Person);

            IsBusy = false;
        }

        private bool CanDelete(object arg)
        {
            return !IsBusy;
        }

        private async void OnDelete(object obj)
        {
            IsBusy = true;
            await SketchManager.DefaultManager.DeleteCurrentUserAsync();
            IsBusy = false;
        }
    }
}