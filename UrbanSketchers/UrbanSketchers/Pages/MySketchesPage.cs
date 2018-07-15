using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using UrbanSketchers.Core;
using UrbanSketchers.Interfaces;
using Xamarin.Forms;

namespace UrbanSketchers.Pages
{
    /// <summary>
    /// My Sketches page
    /// </summary>
	public class MySketchesPage : SketchesPage, IMySketchesPage
	{
        /// <summary>
        /// Initializes a new instance of the MySketchesPage class
        /// </summary>
        /// <param name="sketchManager">the sketch manager</param>
        /// <param name="deleteUserCommand">the delete user command</param>
        /// <param name="downloadCommand">the download command</param>
	    public MySketchesPage(ISketchManager sketchManager,
            IDownloadCommand downloadCommand,
            IDeleteUserCommand deleteUserCommand) : base(sketchManager, downloadCommand, deleteUserCommand)
        {
            Title = Properties.Resources.MySketches;
        }
        /// <summary>
        /// Gets the current user when appearing
        /// </summary>
	    protected override async void OnAppearing()
	    {
            try
            {
	            var person = await SketchManager.GetCurrentUserAsync();

	            if (person != null)
	            {
	                PersonId = person.Id;
	            }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Error appearing my sketches page: {e}");
            }

	        base.OnAppearing();
	    }
	}
}