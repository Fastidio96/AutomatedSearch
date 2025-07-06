using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AutomatedSearch.Model
{
    [XmlRoot("data")]
    public class AppData : INotifyPropertyChanged
    {
        public AppData()
        {
            AppSettings = new AppSettings();
            Accounts = new ObservableCollection<User>();
            Accounts.CollectionChanged += Accounts_CollectionChanged;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void Accounts_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            IReadOnlyList<User> lu = sender as IReadOnlyList<User>;
            if (lu == null || lu.Count == 0)
            {
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                lu[0].PropertyChanged += AppData_PropertyChanged;
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                lu[0].PropertyChanged -= AppData_PropertyChanged;
            }
        }

        private void AppData_PropertyChanged(object? sender, PropertyChangedEventArgs e) //todo: bug here => save BUG!!
        {
            PropertyChanged?.Invoke(sender, e);
        }

        [XmlIgnore]
        public AppSettings AppSettings { get; set; }


        [XmlArray]
        public ObservableCollection<User> Accounts { get; set; }

        [XmlIgnore]
        public User CurrentUser { get; set; }


    }
}
