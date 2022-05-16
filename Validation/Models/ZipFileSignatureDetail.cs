using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipUtility.Validation.Models
{
    public class ZipFileSignatureDetail
    {
        public ZipFileSignatureDetail(string byteString, short byteSize)
        {
            SignatureBytesString = byteString;
            SignatureBytesSize = byteSize;
        }
        public short SignatureBytesSize { get; set; }
        public string SignatureBytesString { get; set; }
    }
}
