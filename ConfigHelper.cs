using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ZipUtility
{
    public static class ConfigHelper
    {
        private static string _sourceFolder;
        private static string _destinationFolder;
        private static string _xmlSchemaValidationFolder;
        private static string[] _validZipContentFileExtensions;
        private static string[] _validZipContentImageFileExtensions;
        private static string _smtpHost;
        private static int _smtpPort;
        private static string _smtpCredentialUserName;
        private static string _smtpCredentialPassword;
        private static string _adminEmailAddress;
        public static string SourceFolder 
        { 
            get 
            {
                if (string.IsNullOrWhiteSpace(_sourceFolder))
                {
                    if(ConfigurationManager.AppSettings["SourceFolder"] != null)
                    {
                        _sourceFolder = ConfigurationManager.AppSettings["SourceFolder"].ToString();
                    }
                }
                return _sourceFolder;
            }             
        }

        public static string DestinationFolder
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_destinationFolder))
                {
                    if (ConfigurationManager.AppSettings["DestinationFolder"] != null)
                    {
                        _destinationFolder = ConfigurationManager.AppSettings["DestinationFolder"].ToString();
                    }
                }
                return _destinationFolder;
            }
        }

        public static string XmlSchemaValidationFolder
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_xmlSchemaValidationFolder))
                {
                    if (ConfigurationManager.AppSettings["SourceXmlValidateFolder"] != null)
                    {
                        _xmlSchemaValidationFolder = ConfigurationManager.AppSettings["SourceXmlValidateFolder"].ToString();
                    }
                }
                return _xmlSchemaValidationFolder;
            }
        }

        public static string[] ValidZipContentFileExtensions
        {
            get
            {
                if (_validZipContentFileExtensions == null || _validZipContentFileExtensions.Length <= 0)
                {
                    if (ConfigurationManager.AppSettings["ValidZipContentFileTypes"] != null)
                    {
                        var validFileExtensions = ConfigurationManager.AppSettings["ValidZipContentFileTypes"].ToString();
                        _validZipContentFileExtensions = validFileExtensions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s=>s.ToLower().Trim()).ToArray();
                    }
                }
                return _validZipContentFileExtensions;
            }
        }

        public static string[] ValidZipContentImageFileExtensions
        {
            get
            {
                if (_validZipContentImageFileExtensions == null || _validZipContentImageFileExtensions.Length<= 0)
                {
                    if (ConfigurationManager.AppSettings["ValidZipContentImageFileTypes"] != null)
                    {
                        var validFileExtensions = ConfigurationManager.AppSettings["ValidZipContentImageFileTypes"].ToString();
                        _validZipContentImageFileExtensions = validFileExtensions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.ToLower().Trim()).ToArray();
                    }
                }
                return _validZipContentImageFileExtensions;
            }
        }

        public static string SmtpHost
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_smtpHost))
                {
                    if (ConfigurationManager.AppSettings["SMTP.HostName"] != null)
                    {
                        _smtpHost = ConfigurationManager.AppSettings["SMTP.HostName"].ToString();
                    }
                }
                return _smtpHost;
            }
        }

        public static int SmtpPort
        {
            get
            {
                if (_smtpPort == 0)
                {
                    if (ConfigurationManager.AppSettings["SMTP.Port"] != null)
                    {
                        _smtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["SMTP.Port"]);
                    }
                }
                return _smtpPort;
            }
        }

        public static string SmtpCredentialUserName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_smtpCredentialUserName))
                {
                    if (ConfigurationManager.AppSettings["SMTP.CredentialUser"] != null)
                    {
                        _smtpCredentialUserName = ConfigurationManager.AppSettings["SMTP.CredentialUser"].ToString();
                    }
                }
                return _smtpCredentialUserName;
            }
        }

        public static string SmtpCredentialPassword
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_smtpCredentialPassword))
                {
                    if (ConfigurationManager.AppSettings["SMTP.CredentialPassword"] != null)
                    {
                        _smtpCredentialPassword = ConfigurationManager.AppSettings["SMTP.CredentialPassword"].ToString();
                    }
                }
                return _smtpCredentialPassword;
            }
        }
        public static string AdminEmailAddress
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_adminEmailAddress))
                {
                    if (ConfigurationManager.AppSettings["Notification.AdminEmailAddress"] != null)
                    {
                        _adminEmailAddress = ConfigurationManager.AppSettings["Notification.AdminEmailAddress"].ToString();
                    }
                }
                return _adminEmailAddress;
            }
        }
    }
}
