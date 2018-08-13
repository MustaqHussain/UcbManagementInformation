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

namespace UcbManagementInformation.MVVM
{
    public abstract class ViewModel : NotifyPropertyChangedEnabledBase,IDisposable
    {
        private bool _isDisposed;
        public void Dispose()
        {
            Dispose(true);

            // Use supressfinalize in case a subclass of this type implements a finalizer
            GC.SuppressFinalize(this);
        }
        public virtual void Initialize()
        { }
        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    //Dispose managed resources
                }
            }
            //Dispose Unmanaged resources
            _isDisposed = true;
        }
    }
}
