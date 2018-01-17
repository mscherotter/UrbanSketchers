using UrbanSketchers.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Pages
{
    /// <summary>
    ///     Sketch comments page
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SketchCommentsPage : ISketchCommentsPage
    {
        private readonly ISketchCommentsPageViewModel _viewModel;

        /// <summary>
        ///     Initializes a new instance of the SketchCommentsPage class.
        /// </summary>
        public SketchCommentsPage()
        {
            InitializeComponent();

            _viewModel = DependencyService.Get<ISketchCommentsPageViewModel>(DependencyFetchTarget.NewInstance);

            _viewModel.Page = this;

            BindingContext = _viewModel;
        }

        /// <summary>
        ///     Gets or sets the sketch Id
        /// </summary>
        public string SketchId
        {
            get => _viewModel.SketchId;
            set => _viewModel.SketchId = value;
        }
    }
}