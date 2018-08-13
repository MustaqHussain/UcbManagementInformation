using System.ComponentModel;

namespace UcbManagementInformation.MVVM
{  public class NotifyPropertyChangedEnabledBase : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
      var handler = this.PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(propertyName));
      }
    }
  }
}