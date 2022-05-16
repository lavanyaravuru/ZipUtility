using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;
using ZipUtility.Validation;
using log4net;
using ZipUtility.Notification;

namespace ZipUtility
{
    public class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        public static void Main(string[] args)
        {
            _log.Debug($"ZipUtility process started at {DateTime.Now}");
            Console.WriteLine($"ZipUtility process started at {DateTime.Now}");

            if (Directory.Exists(ConfigHelper.SourceFolder))
            {
                var sourceFiles = Directory.GetFiles(ConfigHelper.SourceFolder);
                if (sourceFiles == null || sourceFiles.Count() <= 0)
                {
                    // no files found in source directory
                    _log.Debug($"No files found in source folder {ConfigHelper.SourceFolder}");
                    return;
                }

                _log.Debug($"{sourceFiles.Count()} files found in source folder {ConfigHelper.SourceFolder}");

                ZipValidator zipValidator = new ZipValidator(_log);
                EmailService emailService = new EmailService(_log);

                foreach (var sourceFile in sourceFiles)
                {
                    _log.Debug($"Processing source file {sourceFile}");

                    // validate if the input file is a valid zip file and not corrupted
                    // if successful continue with next step, otherwise send a notification to admin about the error and switch next file 
                    if (!zipValidator.IsValidZipFile(sourceFile))
                    {
                        _log.Debug($"Unable to process the source file {sourceFile}. File is not a valid compressed file.");
                        emailService.SendEmail(new EmailModel(NotificationType.Failure, $"Unable to process [{sourceFile}]. Invalid zip file."));
                        continue;
                    }

                    // validate if the files inside the zip are of allowed file types and extract the contents
                    // if successful continue with next step, otherwise send a notification to admin about the error and switch next file
                    if (!zipValidator.ValidateAndExtractContents(sourceFile))
                    {
                        _log.Debug($"Unable to process the source file {sourceFile}. zip file contents are invalid.");
                        emailService.SendEmail(new EmailModel(NotificationType.Failure, $"There is an error validating zip file[{sourceFile}]."));
                        continue;
                    }

                    // notify administrator about process success
                    emailService.SendEmail(new EmailModel(NotificationType.Success, $"Zip file [{sourceFile}] successfully validated and extracted."));
                }
            }
            else
            {
                // source directory not exists
                _log.Debug($"Source folder: {ConfigHelper.SourceFolder} doesn't exists. Please create and copy zip files in the folder.");
            }
            Console.WriteLine($"ZipUtility process completed at {DateTime.Now}");
            Console.ReadLine();
        }
    }
}
