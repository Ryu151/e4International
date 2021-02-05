using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace e4International.Models
{
    [XmlRoot(ElementName = "Entry")]
    public class Entry
    {          
        
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "UserName")]
        public string UserName { get; set; }
        [XmlElement(ElementName = "Surname")]
        public string Surname { get; set; }
        [XmlElement(ElementName = "CellPhone")]
        public string CellPhone { get; set; }
    }
    [XmlRoot(ElementName = "Entries")]
    public class Entries {
        [XmlElement(ElementName = "Entry")]
        public List<Entry> Entry { get; set; }
    }
}
