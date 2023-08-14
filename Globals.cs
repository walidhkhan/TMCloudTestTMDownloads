using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Configuration;

namespace TestTMDownloads
{
    public class Globals
    {
        static Logger _logger = LogManager.GetCurrentClassLogger();
        public static string EMAIL_REPORTS = "reports@tmcloud.com";
        public static string EMAIL_HELP = "support@tmcloud.com";
        public static string EMAIL_HELP_SUBJECT = "TMCloud Help";
        public static string EMAIL_LOUIS = "lstevenson@tmcloud.com";
        public static string EMAIL_ERROR_SUBJECT = "TMCloud Error";

        public static MailMessage GenerateEmail(string to, string cc, string bcc, string subject, string body, string from = "")
        {
            var email = new MailMessage();

            string[] toEmailAddresses = to.Split(new Char[] { ',', ';' });
            foreach (string toEmailAddress in toEmailAddresses)
            {
                email.To.Add(new MailAddress(toEmailAddress.Trim()));
            }

            if (cc != null && cc != String.Empty)
            {
                string[] ccEmailAddresses = cc.Split(new Char[] { ',', ';' });
                foreach (string ccEmailAddress in ccEmailAddresses)
                {
                    email.CC.Add(new MailAddress(ccEmailAddress.Trim()));
                }
            }

            if (bcc != null && bcc != String.Empty)
            {
                string[] bccEmailAddresses = bcc.Split(new Char[] { ',', ';' });
                foreach (string bccEmailAddress in bccEmailAddresses)
                {
                    email.Bcc.Add(new MailAddress(bccEmailAddress.Trim()));
                }
            }
            string emailFrom = from;
            if (emailFrom == null || emailFrom == "")
            {
                //emailFrom = UserContext.GetActiveUserData().Email;
                if (string.IsNullOrWhiteSpace(emailFrom))
                {
                    emailFrom = EMAIL_HELP;
                }
            }

            email.From = new MailAddress(emailFrom);
            email.Subject = subject != null ? subject : string.Empty;

            if (!string.IsNullOrWhiteSpace(body))
            {
                email.Body = body;
            }
            email.IsBodyHtml = true;
            email.BodyEncoding = Encoding.UTF8;

            return email;
        }

        public static bool SendEmail(MailMessage email)
        {
            try
            {
                var smtp = new SmtpClient();
                smtp = SmtpClient(smtp);
                smtp.Send(email);

                email.Dispose();

                return true;
            }
            catch (Exception e)
            {
                _logger.Error("Error Sending Email - " + e.Message + " for email from " + email.From.ToString() + ", to: " + email.To.ToString() + ", subject: " + email.Subject.ToString());
                return false;
            }
        }

        public static SmtpClient SmtpClient(SmtpClient smtp)
        {
            smtp.Host = ConfigurationManager.AppSettings["SMTPHost"];
            smtp.Port = Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]);
            smtp.EnableSsl = (ConfigurationManager.AppSettings["SMTPEnableSSL"].ToLower() == "true");
            if (ConfigurationManager.AppSettings["SMTPUser"] != string.Empty && ConfigurationManager.AppSettings["SMTPPass"] != string.Empty)
            {
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["SMTPUser"], ConfigurationManager.AppSettings["SMTPPass"]);
            }
            else
            {
                smtp.UseDefaultCredentials = true;
            }

            return smtp;
        }

    }
}
