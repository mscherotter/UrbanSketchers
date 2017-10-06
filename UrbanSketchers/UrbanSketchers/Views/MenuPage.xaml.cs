using System.Collections.ObjectModel;
using UrbanSketchers.Commands;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage
    {
        public MenuPage()
        {
            InitializeComponent();

            string sketchMapIcon = null;
            string urbanSketchersIcon = null;
            string sketchesIcon = null;

            switch (Device.RuntimePlatform)
            {
                case Device.UWP:
                    sketchMapIcon = "Assets/SketchMap.png";
                    urbanSketchersIcon = "Assets/UrbanSketchers.png";
                    sketchesIcon = "Assets/Sketches.png";
                    break;

            }

            Items = new ObservableCollection<NavigationMenuItem>(new[]
            {
                new NavigationMenuItem
                {
                    Label =  Properties.Resources.SketchMap,
                    Command = new NavigationCommand<MapPage>("Map"),
                    Icon = sketchMapIcon
                },
                new NavigationMenuItem
                {
                    Label = Properties.Resources.Sketches,
                    Command = new NavigationCommand<SketchesPage>("Sketches"),
                    Icon = sketchesIcon
                    
                },
                new NavigationMenuItem
                {
                    Label = Properties.Resources.UrbanSketchers,
                    Command = new NavigationCommand<PeoplePage>("People"),
                    Icon = urbanSketchersIcon
                },
            });

            BindingContext = this;
        }

        public ObservableCollection<NavigationMenuItem> Items { get; }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is NavigationMenuItem item)
            {
                if (item.Command.CanExecute(null))
                    item.Command.Execute(null);

                if (sender is ListView listView)
                    listView.SelectedItem = null;
            }
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is NavigationMenuItem item)
            {
                if (item.Command.CanExecute(null))
                    item.Command.Execute(null);

                if (sender is ListView listView)
                    listView.SelectedItem = null;
            }
        }
    }
}