using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;

namespace FirstLook.ServiceModel.DomainServices.Client.Security
{
    public class AuthorizationCommand : ICommand
    {
        private readonly AuthorizationSource _source;
        private readonly ICommand _command;

        public AuthorizationCommand(AuthorizationSource source, ICommand command)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            this._source = source;
            this._command = command;

            this._source.PropertyChanged += this.SourcePropertyChanged;
            this._command.CanExecuteChanged += this.CommandCanExecuteChanged;
        }

        private void SourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaiseCanExecuteChanged();
        }

        private void CommandCanExecuteChanged(object sender, EventArgs e)
        {
            this.RaiseCanExecuteChanged();
        }

        public bool CanExecute(object parameter)
        {
            return (this._source.Result == AuthorizationResult.Allowed) && this._command.CanExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        private void RaiseCanExecuteChanged()
        {
            EventHandler handler = this.CanExecuteChanged;
            EventArgs args = new EventArgs();
            if (handler != null)
            {
                handler(this, args);
            }
        }

        public void Execute(object parameter)
        {
            this._command.Execute(parameter);
        }
    }
}
