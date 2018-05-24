using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace UrbanSketchers.Interfaces
{
    public interface IDownloadCommand : ICommand
    {
        string PersonId { get; set; }
    }
}
