using HandMade.Helprer;
using HandMade.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace HandMade.Manager
{
    public class AuthorizationManagement
    {
        private HandMadeContext context = new HandMadeContext();
        private TokenManagement tokenManagement = new TokenManagement();

        public string IsUserLogedIn()
        {


            string userName = "";


            string token = tokenManagement.GetCookieValue();

            if (token == null || token == "") return "";

            if (tokenManagement.GetClaimValueFromToken("User", token) != null)
            {
                userName = tokenManagement.GetClaimValueFromToken("User", token);
            }

            if (!string.IsNullOrEmpty(userName))
            {
                Account account = context.Accounts.FirstOrDefault(a => a.Email == userName);

                if (account == null) return "";
                if (account.Token != token) return "";

                return account.Email;

            }

            return "";
        }

        public void SendEmail(string to, string title, string body)
        {
            MailMessage message = new MailMessage(
                   "fromemail@contoso.com",
                   to,
                   title,
                   body);

            message.IsBodyHtml = true;

            SmtpClient client = new SmtpClient();
            client.Send(message);
        }
    }
}