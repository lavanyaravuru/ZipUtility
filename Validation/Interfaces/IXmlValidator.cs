using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZipUtility.Validation.Models;

namespace ZipUtility.Validation.Interfaces
{
    public interface IXmlValidator
    {
        bool ValidateXml(string xmlfilePath, string xsdPath);
        Party DeserializeXml(string xmlInput);
    }
}
