using System;
using UrbanSketchers.Views;
using Xamarin.Forms;

namespace UrbanSketchers
{
	public class App : Application
	{
		public App ()
		{
			// The root page of your application
			MainPage = new TabbedPage 
			{
			    Children =
			    {
			        new NavigationPage(new MapPage())
			        {
			            Title = "Sketch Map",
			            Icon = Device.OnPlatform<string>("tab_feed.png",null,null)
			        },
			        new NavigationPage(new SketchesPage())
			        {
			            Title = "Sketches",
			            Icon = Device.OnPlatform<string>("tab_about.png",null,null)
			        },
			        new NavigationPage(new PeoplePage())
			        {
			            Title = "Urban Sketchers",
			            Icon = Device.OnPlatform<string>("tab_about.png",null,null)
			        },
			    }
			};
        }

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

