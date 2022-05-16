using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ZipUtility.Validation.Interfaces;
using System.IO.Compression;
using System.Xml.Serialization;
using ZipUtility.Validation.Models;
using System.Xml;
using log4net;

namespace ZipUtility.Validation
{
    public class ZipValidator :IZipValidator
    {
        private static List<ZipFileSignatureDetail> zipFileSignatures;
        private static string[] validFileExtensions;
        private static ILog _logger;

        public ZipValidator(ILog logger)
        {
            zipFileSignatures = new List<ZipFileSignatureDetail>
            {
                new ZipFileSignatureDetail("50-4B-03-04",4),      //PKzip
                new ZipFileSignatureDetail("50-4B-05-06",4),      //PKzip
                new ZipFileSignatureDetail("50-4B-07-08",4),      //PKzip
                new ZipFileSignatureDetail("1F-8B",2),            //Gzip,
                new ZipFileSignatureDetail("1F-9D",2),            //Tarzip
                new ZipFileSignatureDetail("1F-A0",2),            //Tarzip
                new ZipFileSignatureDetail("42-5A-68", 3),        //Bzip2
                new ZipFileSignatureDetail("4C-5A-49-50",4),      //Lzip
                new ZipFileSignatureDetail("37-7A-BC-AF-27-1C",6) //7-zip
            };
            
            validFileExtensions = ConfigHelper.ValidZipContentFileExtensions
                .Union(ConfigHelper.ValidZipContentImageFileExtensions)
                .ToArray();

            _logger = logger;
        }

        /// <summary>
        /// Validates if the source file is of type zip
        /// Validate the actual zip file signaure to make sure the file is a valid compressed file and not corrupted
        /// </summary>
        /// <param name="filePath">source zip file path</param>
        /// <returns></returns>
        public bool IsValidZipFile(string filePath)
        {
            // check the file extension to identify if it a zip file or not
            if (IsZipExtension(filePath))
            {
                try
                {
                    // validate if the file is valid compressed file. check the signature bytes
                    foreach (var signatureBytesGroup in zipFileSignatures.GroupBy(g => g.SignatureBytesSize))
                    {
                        var HeaderBytesToRetreive = signatureBytesGroup.Key;
                        var headerBytes = GetHeaderBytesBySize(filePath, HeaderBytesToRetreive);
                        var test = BitConverter.ToString(headerBytes);
                        if (signatureBytesGroup.Any(s => s.SignatureBytesString == BitConverter.ToString(headerBytes)))
                        {
                            _logger.Debug($"{filePath} is a valid compressed file.");
                            return true;
                        }
                    }
                }
                catch(Exception ex)
                {
                    _logger.Error($"Error in IZipValidator.IsValidZipFile. {ex.InnerException}. StackTrace: {ex.StackTrace}");
                }
            }
            else
            {
                _logger.Debug($"{filePath} is not a .zip file");
            }
            return false;
        }


        /// <summary>
        /// This method validates the contents inside the zip file
        /// Checks if all the files are of allowed file types
        /// Validates party.XML against party.XSD
        /// Extracts the contents to folder
        /// </summary>
        /// <param name="filePath">source zip file path</param>
        /// <returns></returns>
        public bool ValidateAndExtractContents(string filePath)
        {
            try
            {
                var validFileExtensions = ConfigHelper.ValidZipContentFileExtensions.Union(ConfigHelper.ValidZipContentImageFileExtensions).ToArray();
                var xmlFileToValidate = string.Empty;
                var xsdFile = string.Empty;

                //check if the files inside zip are allowed file types
                using (var zipFile = ZipFile.OpenRead(filePath))
                {
                    var entries = zipFile.Entries;
                    foreach (ZipArchiveEntry entry in zipFile.Entries)
                    {
                        var fileExtension = GetFileExtension(entry.FullName);
                        if (!validFileExtensions.Contains(fileExtension))
                        {
                            _logger.Error($"{fileExtension} extension is not an allowed file type to be included in zip file contents.");
                            return false;
                        }

                        switch (fileExtension)
                        {
                            case "xml":
                                xmlFileToValidate = Path.Combine(ConfigHelper.XmlSchemaValidationFolder, entry.FullName);
                                entry.ExtractToFile(xmlFileToValidate, true);
                                break;
                            case "xsd":
                                xsdFile = Path.Combine(ConfigHelper.XmlSchemaValidationFolder, entry.FullName);
                                entry.ExtractToFile(xsdFile, true);
                                break;
                        }
                    }

                    // if the zip file contains XML, validate it agains XSD
                    XmlValidator xmlValidator = new XmlValidator(_logger);
                    Party partyXml = null;
                    if(xmlValidator.ValidateXml(xmlFileToValidate, xsdFile))
                    {
                        partyXml = xmlValidator.DeserializeXml(xmlFileToValidate);
                    }
                    else
                    {
                        _logger.Error($"Party.XML extension is not an allowed file type to be included in zip file contents.");
                        return false;  
                    }

                    if (partyXml == null)
                    {
                        _logger.Debug($"Party.XML file {xmlFileToValidate} is not valididated against Party.XSD {xsdFile}");
                        return false;
                    }

                    var extractedFolderName = Path.Combine(ConfigHelper.DestinationFolder, $"{partyXml.ApplicationNo}-{Guid.NewGuid()}");
                    ZipFile.ExtractToDirectory(filePath, extractedFolderName);
                    _logger.Debug($"{filePath} extracted to {extractedFolderName}");
                    return true;
                }
            }
            catch (InvalidDataException ex)
            {
                //log and return
                _logger.Error($"Source file {filePath} contents are invalid. Inner Exception: {ex.InnerException}. Stack Trace: {ex.StackTrace} ");
                return false;
            }
            catch (Exception ex)
            {
                //log and return
                _logger.Error($"Source file {filePath} contents are invalid. Inner Exception: {ex.InnerException}. Stack Trace: {ex.StackTrace} ");
                return false;
            }
        }

        private bool IsZipExtension(string filePath)
        {
            return Path.GetExtension(filePath).Equals(".zip");
        }             

        private byte[] GetHeaderBytesBySize(string filepath, int length)
        {
            using (var reader = new StreamReader(filepath))
            {
                reader.BaseStream.Seek(0, 0);
                var bytes = new byte[length];
                reader.BaseStream.Read(bytes, 0, length);

                return bytes;
            }
        }

        private string GetFileExtension(string fileName)
        {
            int lastIndex = fileName.LastIndexOf('.');
            return fileName.Substring(lastIndex + 1).ToLower();
        }
    }
}
