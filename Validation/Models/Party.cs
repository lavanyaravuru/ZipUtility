using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ZipUtility.Validation.Models
{
    [Serializable, XmlRoot("party")]
    public class Party
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "email")]
        public string Email { get; set; }

        [XmlElement(ElementName = "applicationno")]
        public int ApplicationNo { get; set; }
    }
}
