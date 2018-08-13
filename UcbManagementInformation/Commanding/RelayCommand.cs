using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace UcbManagementInformation.Commanding
{
    public class RelayCommand<T> : ICommand
    {
        #region Fields

        readonly Action<T> _execute = null;
        readonly Predicate<T> _canExecute = null;

        #endregion // Fields

        #region Constructors

        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion // Constructors
        public void UpdateCanExecuteCommand()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }
        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            if (!(parameter is T))
            {
                return false;
            }
            return _canExecute == null ? true : _canExecute((T)parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }
        protected void OnCanExecuteChanged(object sender, EventArgs args)
        {
            var handler = this.CanExecuteChanged;
            if (handler != null)
            {
                handler(sender, args);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            this.OnCanExecuteChanged(this, EventArgs.Empty);
        }
        #endregion // ICommand Members
    }
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;


        public RelayCommand(
                            Action<object> execute,
                            Predicate<object> canExecute = null)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }


        public void UpdateCanExecuteCommand()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }


        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }


        public void Execute(object parameter)
        {
            _execute(parameter);
        }
        protected void OnCanExecuteChanged(object sender, EventArgs args)
        {
            var handler = this.CanExecuteChanged;
            if (handler != null)
            {
                handler(sender, args);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            this.OnCanExecuteChanged(this, EventArgs.Empty);
        }
        
    }

}
    /*
To use this, create a property where the Command property of the control will be bounded. Also, the execute and canExecute methods should accept a parameter.

public ICommand MyCommand
{
    get
    {
        if (myCommand == null)
        {
             myCommand = new RelayCommand<object>(CommandExecute, CanCommandExecute);
        }

        return myCommand;
    }
}

private void CommandExecute(object parameter)
{
    // Code here
}

private bool CanCommandExecute(object parameter)
{
    // Code here
    return true;
}


}
*/