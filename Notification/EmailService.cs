using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ZipUtility.Notification
{
    public class EmailService
    {
        private static ILog _logger;
        public EmailService(ILog logger)
        {
            _logger = logger;
        }
        public void SendEmail(EmailModel emailModel)
        {
            _logger.Debug($"Sending notification to administrator.");

            try
            {
                MailMessage message = new MailMessage()
                {
                    From = new MailAddress(ConfigHelper.SmtpCredentialUserName),
                    Subject = emailModel.Subject,
                    IsBodyHtml = true, //to make message body as html  
                    Body = emailModel.Body
                };

                foreach(var toAddress in emailModel.To)
                {
                    message.To.Add(toAddress);
                }

                SmtpClient smtp = new SmtpClient()
                {
                    Port = ConfigHelper.SmtpPort,
                    Host = ConfigHelper.SmtpHost,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(ConfigHelper.SmtpCredentialUserName, ConfigHelper.SmtpCredentialPassword),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };
                smtp.Send(message);
            }
            catch (Exception ex) 
            {
                _logger.Error($"Error Sending notification to administrator. Exception: {ex.InnerException}. StackTrace: {ex.StackTrace}");
            }
        }
    }
}
