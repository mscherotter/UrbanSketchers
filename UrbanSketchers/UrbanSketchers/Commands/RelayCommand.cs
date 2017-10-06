// <copyright file="RelayCommand.cs" company="Michael S. Scherotter">
// Copyright (c) 2013 Michael S. Scherotter All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>synergist@charette.com</email>
// <date>2013-10-14</date>
// <summary>Relay Command</summary>

using System;
using System.Windows.Input;

namespace UrbanSketchers.Commands
{
    /// <summary>
    ///     A command whose sole purpose is to relay its functionality
    ///     to other objects by invoking delegates.
    ///     The default return value for the CanExecute method is 'true'.
    ///     <see cref="RaiseCanExecuteChanged" /> needs to be called whenever
    ///     <see cref="CanExecute" /> is expected to return a different value.
    /// </summary>
    /// <typeparam name="T">the parameter type</typeparam>
    public class RelayCommand<T> : ICommand
    {
        /// <summary>
        ///     can execute function
        /// </summary>
        private readonly Func<T, bool> canExecute;

        /// <summary>
        ///     execute action
        /// </summary>
        private readonly Action<T> execute;

        public RelayCommand()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the RelayCommand class.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the RelayCommand class.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        /// <summary>
        ///     Gets the command parameter
        /// </summary>
        public object Parameter { get; private set; }

        /// <summary>
        ///     Raised when RaiseCanExecuteChanged is called.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        ///     Determines whether this <see cref="RelayCommand" /> can execute in its current state.
        /// </summary>
        /// <param name="parameter">
        ///     Data used by the command. If the command does not require data to be passed, this object can be set to null.
        /// </param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute((T) parameter);
        }

        /// <summary>
        ///     Executes the <see cref="RelayCommand" /> on the current command target.
        /// </summary>
        /// <param name="parameter">
        ///     Data used by the command. If the command does not require data to be passed, this object can be set to null.
        /// </param>
        public void Execute(object parameter)
        {
            execute((T) parameter);
        }

        /// <summary>
        ///     Method used to raise the <see cref="CanExecuteChanged" /> event
        ///     to indicate that the return value of the <see cref="CanExecute" />
        ///     method has changed.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}