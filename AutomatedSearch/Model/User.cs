using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace AutomatedSearch.Model
{
    [XmlRoot("user")]
    public class User : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public User()
        {
            SerializableCookies = new List<SerializableCookie>();
            Cookies = new CookieContainer();
        }


        [XmlArray("Cookies"), XmlArrayItem(typeof(SerializableCookie), ElementName = "Cookie")]
        public List<SerializableCookie> SerializableCookies
        {
            get
            {
                return _serializableCookies;
            }
            set
            {
                if (_serializableCookies != value)
                {
                    _serializableCookies = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private List<SerializableCookie> _serializableCookies;

        [XmlIgnore]
        public CookieContainer Cookies
        {
            get
            {
                SerializableCookies.Clear();
                foreach (Cookie cookie in _cookies.GetAllCookies())
                {
                    SerializableCookies.Add(new SerializableCookie
                    (
                        cookie.Name,
                        cookie.Value,
                        cookie.Path,
                        cookie.Domain,
                        cookie.Expires,
                        cookie.Expired,
                        cookie.HttpOnly,
                        cookie.Secure
                    ));
                }

                return _cookies;
            }
            set
            {
                if (_cookies != value)
                {
                    _cookies = new CookieContainer();
                    SerializableCookies.ForEach(c => _cookies.Add(c.GetCookie()));
                }
            }
        }
        private CookieContainer _cookies;


        [XmlElement]
        public UserStatus Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private UserStatus _status;


        [XmlElement]
        public string Username
        {
            get => _userName;
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _userName;

        [XmlElement]
        public Int32 TotalPoints
        {
            get => _totalPoints;
            set
            {
                if (_totalPoints != value)
                {
                    _totalPoints = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private Int32 _totalPoints;


        [XmlElement]
        public Int32 CurrentDailySearch
        {
            get => _currentDailySearch;
            set
            {
                if (_currentDailySearch != value)
                {
                    _currentDailySearch = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private Int32 _currentDailySearch;


        [XmlElement]
        public Int32 MaxDailySearch
        {
            get => _maxDailySearch;
            set
            {
                if (_maxDailySearch != value)
                {
                    _maxDailySearch = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private Int32 _maxDailySearch;

        /// <summary>
        /// Last time when the account infos was checked/updated
        /// </summary>
        [XmlElement]
        public DateTime LastUpdate
        {
            get => _lastUpdate;
            set
            {
                if (_lastUpdate != value)
                {
                    _lastUpdate = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private DateTime _lastUpdate;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            //System.Diagnostics.Debug.WriteLine(string.Format("Updated property {0}", propertyName));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum UserStatus : byte
    {
        Unknown = 0,
        NotLogged = 1,
        Logged = 2,
        Completed = 3,
        Required2FA = 4
    }
}
