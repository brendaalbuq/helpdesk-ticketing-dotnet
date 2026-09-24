//-----------------------------------------------------------------
//    <copyright file="RelayCommand.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>13-07-2026</date>
//    <time>19:10</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

using System.Windows.Input;

namespace Helpdesk.WpfApp.ViewModels.Commands
{
    /// <summary>
    /// Represents a reusable WPF command.
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region Fields

        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when command execution availability changes.
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The execute action.</param>
        /// <param name="canExecute">The can execute predicate.</param>
        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks if the command can execute.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>True if it can execute; otherwise, false.</returns>
        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public void Execute(object? parameter)
        {
            _execute(parameter);
        }

        #endregion
    }
}

