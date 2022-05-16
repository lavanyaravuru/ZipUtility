using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ZipUtility.Validation.Models;
using System.IO.Compression;
using ZipUtility.Validation;
using System.Xml.Serialization;
using System.Xml;

namespace ZipUtility
{
    public static class ZipHelper
    {
        private static readonly IDictionary<string, short> zipSignatureBytes = new Dictionary<string, short>
        {
            { "50-4B-03-04",4},      //PKzip
            { "50-4B-05-06",4},      //PKzip
            { "50-4B-07-08",4},      //PKzip
            { "1F-8B",2},            //Gzip
            { "1F-9D",2},            //Tar zip
            { "1F-A0",2},            //Tar zip
            { "42-5A-68",3},         //Bzip2
            { "4C-5A-49-50",4},      //Lzip
            { "37-7A-BC-AF-27-1C",6} //7-zip
        };
        private static readonly byte[] PKzipBytes1 = { 0x50, 0x4b, 0x03, 0x04, 0x0a }; //PKzip
        private static readonly byte[] PKzipBytes2 = { 0x50, 0x4b, 0x05, 0x06 }; //PKzip
        private static readonly byte[] PKzipBytes3 = { 0x50, 0x4b, 0x07, 0x08 }; //PKzip
        private static readonly byte[] GzipBytes = { 0x1f, 0x8b }; //Gzip zip
        private static readonly byte[] TarBytes = { 0x1f, 0x9d };  //tar zip
        private static readonly byte[] LzhBytes = { 0x1f, 0xa0 }; //tar zip
        private static readonly byte[] Bzip2Bytes = { 0x42, 0x5a, 0x68 };  //Bzip2
        private static readonly byte[] LzipBytes = { 0x4c, 0x5a, 0x49, 0x50 }; //Lzip
        private static readonly byte[] zip7Bytes = { 0x37, 0x7a, 0xbc, 0xaf, 0x27, 0x1c }; //7-zip

        public static bool IsValidZipFile(string filePath)
        {
            if (filePath.HasZipExtension())
            {
                return IsCompressedData(GetFirstBytes(filePath, 5));
            }
            return false;
        }
        private static bool HasZipExtension(this string filePath)
        {
            return Path.GetExtension(filePath).Equals(".zip");
        }


        private static List<ZipFileSignatureDetail> GetZipFileSignatures()
        {
            //return zipSignatureBytes.Select(x => new ZipFileSignatureDetail(x.Key,x.Value)).ToList();
            return new List<ZipFileSignatureDetail>
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
        }
        /*
        public static bool ValidateZipContents(string filePath)
        {
            try
            {
                var validFileExtensions = ConfigHelper.ValidZipContentFileExtensions.Union(ConfigHelper.ValidZipContentImageFileExtensions).ToArray();
                var xmlFileToValidate = string.Empty;
                var xsdFile = string.Empty;

                using (var zipFile = ZipFile.OpenRead(filePath))
                {

                    var entries = zipFile.Entries;
                    foreach (ZipArchiveEntry entry in zipFile.Entries)
                    {
                        var fileExtension = GetFileExtension(entry.FullName);
                        if (!validFileExtensions.Contains(fileExtension))
                        {
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

                    XmlValidator xmlValidator = new XmlValidator();
                    if (!xmlValidator.ValidateXml(xmlFileToValidate, xsdFile))
                    {
                        return false;
                    }

                    XmlSerializer ser = new XmlSerializer(typeof(Party));
                    using (XmlReader reader = XmlReader.Create(xmlFileToValidate))
                    {
                        //cars = (Cars)ser.Deserialize(reader);
                    }
                    ZipFile.ExtractToDirectory(filePath, ConfigHelper.DestinationFolder);
                    return true;
                }
            }
            catch (InvalidDataException)
            {
                //
            }
            catch (Exception ex)
            {

            }

            return true;
        }
        */

        private static byte[] GetFirstBytes(string filepath, int length)
        {
            using (var sr = new StreamReader(filepath))
            {
                sr.BaseStream.Seek(0, 0);
                var bytes = new byte[length];
                sr.BaseStream.Read(bytes, 0, length);

                return bytes;
            }
        }

        private static bool IsCompressedData(byte[] data)
        {
            foreach (var headerBytes in new[] { PKzipBytes1, PKzipBytes2, PKzipBytes3, GzipBytes, TarBytes, LzhBytes, Bzip2Bytes, LzipBytes, zip7Bytes })
            {
                if (HeaderBytesMatch(headerBytes, data))
                    return true;
            }

            return false;
        }

        private static bool HeaderBytesMatch(byte[] headerBytes, byte[] dataBytes)
        {
            if (dataBytes.Length < headerBytes.Length)
                throw new ArgumentOutOfRangeException(nameof(dataBytes),
                    $"Passed databytes length ({dataBytes.Length}) is shorter than the headerbytes ({headerBytes.Length})");

            for (var i = 0; i < headerBytes.Length; i++)
            {
                if (headerBytes[i] == dataBytes[i]) continue;

                return false;
            }

            return true;
        }

        //private void ValidateZipContents()
        //{

        //}

        /*
         * bool isPKZip = IOHelper.CheckSignature(pkg, 4, IOHelper.SignatureZip);
        Assert.IsTrue(isPKZip, "Not ZIP the package : " + pkg);

// http://blog.somecreativity.com/2008/04/08/how-to-check-if-a-file-is-compressed-in-c/
    public static partial class IOHelper
    {
        public const string SignatureGzip = "1F-8B-08";
        public const string SignatureZip = "50-4B-03-04";

        public static bool CheckSignature(string filepath, int signatureSize, string expectedSignature)
        {
            if (String.IsNullOrEmpty(filepath)) throw new ArgumentException("Must specify a filepath");
            if (String.IsNullOrEmpty(expectedSignature)) throw new ArgumentException("Must specify a value for the expected file signature");
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (fs.Length < signatureSize)
                    return false;
                byte[] signature = new byte[signatureSize];
                int bytesRequired = signatureSize;
                int index = 0;
                while (bytesRequired > 0)
                {
                    int bytesRead = fs.Read(signature, index, bytesRequired);
                    bytesRequired -= bytesRead;
                    index += bytesRead;
                }
                string actualSignature = BitConverter.ToString(signature);
                if (actualSignature == expectedSignature) return true;
                return false;
            }
        }

    }
         */
    }
}
