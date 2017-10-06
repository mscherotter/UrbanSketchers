using System;
using System.Windows.Input;
using UrbanSketchers.Data;

namespace UrbanSketchers
{
    /// <summary>
    /// Navigation menu item
    /// </summary>
    public class NavigationMenuItem : BaseDataObject
    {
        private ICommand _command;

        private bool _isEnabled = true;

        /// <summary>
        /// Gets or sets the label
        /// </summary>
        public string Label { get; set; }

        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the command
        /// </summary>
        public ICommand Command
        {
            get => _command;
            set
            {
                if (_command != null)
                {
                    _command.CanExecuteChanged -= _command_CanExecuteChanged;
                }

                if (SetProperty(ref _command, value) && _command != null)
                    _command.CanExecuteChanged += _command_CanExecuteChanged;
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        private void _command_CanExecuteChanged(object sender, EventArgs e)
        {
            IsEnabled = Command != null && Command.CanExecute(null);
        }
    }
}