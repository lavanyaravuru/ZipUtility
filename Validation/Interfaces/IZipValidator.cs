using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipUtility.Validation.Interfaces
{
    public interface IZipValidator
    {
        bool IsValidZipFile(string filePath);
        bool ValidateAndExtractContents(string filePath);
    }
}
