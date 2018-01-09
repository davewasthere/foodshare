using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Foodshare.Helper
{
    public class EmailHelper
    {
        public static void SendEmail(string to, string subject, string body)
        {
            var message = new MailMessage();
            message.To.Add(to);

            message.From = new MailAddress("no-reply@donations.bendigofoodshare.org.au", "Bendigo Foodshare Donations");

            message.Subject = subject;

            message.Body = body;
            message.IsBodyHtml = true;

            using (var smtpClient = new SmtpClient())
            {

                // I've learnt my lesson with public repos. ;)
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["emailUsername"], ConfigurationManager.AppSettings["emailPassword"]);
                smtpClient.Credentials = credentials;

                smtpClient.Send(message);
            }
        }
    }
}