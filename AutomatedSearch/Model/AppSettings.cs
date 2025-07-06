using System.Collections.Generic;
using System.ComponentModel;

namespace AutomatedSearch.Model
{
    public class AppSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public AppSettings()
        {
        }

        public bool ServiceInstall
        {
            get => _serviceInstall;
            set
            {
                _serviceInstall = value;
                NotifyPropertyChanged(nameof(_serviceInstall));
            }
        }
        private bool _serviceInstall;


        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
