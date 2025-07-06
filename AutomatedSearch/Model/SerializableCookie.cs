using System;
using System.Net;
using System.Xml.Serialization;

namespace AutomatedSearch.Model
{
    [XmlRoot("Cookie")]
    [XmlType("SerializableCookie")]
    public class SerializableCookie
    {
        [XmlElement]
        string Name { get; set; }

        [XmlElement]
        string Value { get; set; }

        [XmlElement]
        string Path { get; set; }

        [XmlElement]
        string Domain { get; set; }

        [XmlElement]
        DateTime Expires { get; set; }

        [XmlElement]
        bool Expired { get; set; }

        [XmlElement]
        bool HttpOnly { get; set; }

        [XmlElement]
        bool IsSecure { get; set; }

        public SerializableCookie(string name, string value, string path, string domain, DateTime expires, bool expired, bool httpOnly, bool isSecure)
        {
            Name = name;
            Value = value;
            Path = path;
            Domain = domain;
            Expires = expires;
            Expired = expired;
            HttpOnly = httpOnly;
            IsSecure = isSecure;
        }

        public SerializableCookie()
        {
        }

        public Cookie GetCookie()
        {
            return new Cookie(Name, Value, Path, Domain)
            {
                Expires = this.Expires,
                Expired = this.Expired,
                HttpOnly = this.HttpOnly,
                Secure = this.IsSecure
            };
        }
    }
}
