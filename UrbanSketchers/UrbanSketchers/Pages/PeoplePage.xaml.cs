﻿using System;
using System.Threading.Tasks;
using Autofac;
using UrbanSketchers.Data;
using UrbanSketchers.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers.Pages
{
    /// <summary>
    ///     People page
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PeoplePage : IPeoplePage
    {
        /// <summary>
        ///     Initializes a new instance of the PeoplePage class.
        /// </summary>
        public PeoplePage()
        {
            InitializeComponent();

            BindingContext = this;
        }

        /// <summary>
        ///     Refresh the people
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                await RefreshAsync();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private async Task RefreshAsync()
        {
            var people = await Core.Container.Current.Resolve<ISketchManager>().GetPeopleAsync();

            People.ItemsSource = people;
        }

        private async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is IPerson person)
                await Navigation.PushAsync(new PersonPage
                {
                    PersonId = person.Id
                }, true);
        }
    }
}