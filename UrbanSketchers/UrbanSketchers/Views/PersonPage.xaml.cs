using System;
using UrbanSketchers.Commands;
using UrbanSketchers.Data;
using UrbanSketchers.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Views
{
    /// <summary>
    /// Person page
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonPage
    {
        private readonly ViewModel _viewModel = new ViewModel();

        /// <summary>
        /// Initializes a new instance of the PersonPage class.
        /// </summary>
        public PersonPage()
        {
            InitializeComponent();

            _viewModel.RefreshCommand = new RelayCommand<object>(OnRefresh, CanRefresh);
            _viewModel.UpdateCommand = new RelayCommand<object>(OnUpdate, CanUpdate);
            _viewModel.DeleteCommand = new RelayCommand<object>(OnDelete, CanDelete);
            BindingContext = _viewModel;
        }

        private bool CanDelete(object arg)
        {
            return !_viewModel.IsBusy;
        }

        private async void OnDelete(object obj)
        {
            _viewModel.IsBusy = true;
            await SketchManager.DefaultManager.DeleteCurrentUserAsync();
            _viewModel.IsBusy = false;
        }

        private bool CanUpdate(object arg)
        {
            return !_viewModel.IsBusy;
        }

        private async void OnUpdate(object obj)
        {
            _viewModel.IsBusy = true;

            await SketchManager.DefaultManager.SaveAsync(_viewModel.Person);

            _viewModel.IsBusy = false;
        }

        private bool CanRefresh(object arg)
        {
            return !_viewModel.IsBusy;
        }

        private async void OnRefresh(object obj)
        {
            _viewModel.IsBusy = true;

            await RefreshAsync();

            _viewModel.IsBusy = false;
        }

        /// <summary>
        /// Gets or sets the person Id
        /// </summary>
        public string PersonId { get; set; }

        /// <summary>
        /// Sets the binding context to the person when appearing
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await RefreshAsync();
        }

        private async System.Threading.Tasks.Task RefreshAsync()
        {
            _viewModel.Person = await SketchManager.DefaultManager.GetPersonAsync(PersonId);

            var sketches = await SketchManager.DefaultManager.GetSketchsAsync(PersonId);

            Sketches.ItemsSource = sketches;
        }

        private async void OnSketchSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is Sketch sketch)
            {
                // This won't work because the Xamarain x:Name "Image" isn't put onto the UWP Xaml Image
                //var data = new Controls.PrepareConnectedAnimationData("image2", sketch, "Sketch");

                //ConnectedAnimations.SetPrepareConnectedAnimation(Sketches, data);

                await Navigation.PushAsync(new SketchPage
                {
                    SketchId = sketch.Id
                }, true);
            }
        }

        public class ViewModel : ObservableObject
        {
            private Person _person;
            private RelayCommand<object> _refreshCommand;
            private bool _isBusy;

            public Person Person
            {
                get
                {
                    return _person;
                }
                set
                {
                    SetProperty(ref _person, value);
                }
            }

            public RelayCommand<object> RefreshCommand
            {
                get
                {
                    return _refreshCommand;
                }
                set
                {
                    SetProperty(ref _refreshCommand, value);
                }
            }

            public bool IsBusy
            {
                get
                {
                    return _isBusy;
                }
                internal set
                {
                    if (SetProperty(ref _isBusy, value))
                    {
                        RefreshCommand?.RaiseCanExecuteChanged();
                        UpdateCommand?.RaiseCanExecuteChanged();
                    }
                }
            }

            public RelayCommand<object> UpdateCommand { get; internal set; }
            public RelayCommand<object> DeleteCommand { get; internal set; }
        }
    }
}