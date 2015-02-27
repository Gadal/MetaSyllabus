using System;
using System.Windows.Input;

namespace MetaSyllabus.Commands
{
    class SimpleCommand : ICommand
    {
        #region Public Members
        public delegate void ICommandOnExecute(object parameter);
        public delegate bool ICommandCanExecute(object parameter);
        #endregion // Public Members

        #region Constructors
        public SimpleCommand(ICommandOnExecute execute, ICommandCanExecute canExecute)
        {
            _execute    = execute;
            _canExecute = canExecute;
        }
        #endregion // Constructors

        #region Implementing the ICommand Interface

        public event EventHandler CanExecuteChanged
        {
            add    { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        private ICommandOnExecute _execute;
        public void Execute(object parameter)
        {
            _execute.Invoke(parameter);
        }

        private ICommandCanExecute _canExecute;
        public bool CanExecute(object parameter)
        {
            return _canExecute.Invoke(parameter);
        }

        #endregion // Implementing the ICommand Interface
    }
}
