using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Schema;
using System.Xml;
using System.Xml.Linq;
using ZipUtility.Validation.Models;
using System.Xml.Serialization;
using log4net;
using ZipUtility.Validation.Interfaces;

namespace ZipUtility.Validation
{
    public class XmlValidator : IXmlValidator
    {
        private static ILog _logger;
        public XmlValidator(ILog logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Validates xml against xsd
        /// </summary>
        /// <param name="xmlfilePath">party.XML path</param>
        /// <param name="xsdPath">party.XSD path</param>
        /// <returns></returns>
        public bool ValidateXml(string xmlfilePath, string xsdPath)
        {
            _logger.Debug($"Validating Party.XML {xmlfilePath} against Party.XSD {xsdPath}.");

            try
            {                
                XmlSchemaSet schema = new XmlSchemaSet();
                schema.Add("", xsdPath);
                XmlReader rd = XmlReader.Create(xmlfilePath);
                XDocument doc = XDocument.Load(rd);
                doc.Validate(schema, ValidationEventHandler);
                _logger.Debug($"Party.XML {xmlfilePath} validated successfully against Party.XSD {xsdPath}.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error validating Party.XML {xmlfilePath} against Party.XSD {xsdPath}. Exception: {ex.InnerException} {ex.StackTrace}");
                return false;
            }
        }
        private void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            XmlSeverityType type = XmlSeverityType.Warning;
            if (Enum.TryParse<XmlSeverityType>("Error", out type))
            {
                if (type == XmlSeverityType.Error) throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Deserializes party XML to Party object
        /// </summary>
        /// <param name="xmlInput">party.XML path</param>
        /// <returns></returns>
        public Party DeserializeXml(string xmlInput)
        {
            _logger.Debug($"Deseralizing Party.XML {xmlInput}.");

            Party partyXml = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Party));
                using (XmlReader reader = XmlReader.Create(xmlInput))
                {
                    partyXml = (Party)serializer.Deserialize(reader);
                }
            }
            catch(Exception ex)
            {
                _logger.Error($"Error valideserializing Party.XML {xmlInput}. Exception: {ex.InnerException} {ex.StackTrace}");
            }
            return partyXml;
        }
    }
}
