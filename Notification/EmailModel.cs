using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipUtility.Notification
{
    public class EmailModel
    {
        public EmailModel(NotificationType notificationType, string emailMessage, string subject = null, string toEmailAddress = null)
        {
            // set default subject if not provided
            if (string.IsNullOrWhiteSpace(subject))
            {
                Subject = notificationType == NotificationType.Success
                    ? "ZipUtility process success notification"
                    : "ZipUtility process failure notification";
            }
            else
            {
                Subject = subject;
            }

            // set To address as admin email address  if not provided
            if (string.IsNullOrWhiteSpace(toEmailAddress))
            {
                toEmailAddress = ConfigHelper.AdminEmailAddress;                
            }
            To = new List<string> { toEmailAddress };

            if (!string.IsNullOrWhiteSpace(emailMessage))
            {
                emailMessage += notificationType == NotificationType.Success 
                    ? "All contents are successfully extracted to the extracted directory. " 
                    : "Please check the exception details in the log file.";
                Body = $"Dear Administrator <br/><br/> {emailMessage}<br/>";
            }
        }
        public string From { get; set; }
        public List<string> To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public NotificationType NotificationType { get; set; }
    }

    public enum NotificationType
    {
        Success =1,
        Failure
    }
}
