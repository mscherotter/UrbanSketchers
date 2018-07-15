using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrbanSketchers
{
    /// <summary>
    /// Shared resources
    /// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SharedResources : ResourceDictionary
	{
        /// <summary>
        /// Initializes a new instance of the SharedResources class.
        /// </summary>
		public SharedResources ()
		{
			InitializeComponent ();
		}
	}
}