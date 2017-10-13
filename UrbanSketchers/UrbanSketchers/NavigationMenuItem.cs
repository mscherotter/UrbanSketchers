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

        private string _label;

        private string _icon;

        private bool _isEnabled = true;

        /// <summary>
        /// Gets or sets the label
        /// </summary>
        public string Label {
            get { return _label; } set { SetProperty(ref _label, value); } }

        public string Icon {
            get { return _icon; }
            set { SetProperty(ref _icon, value); } }

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

        /// <summary>
        /// Gets or sets a value indicating whether the menu item is enabled.
        /// </summary>
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