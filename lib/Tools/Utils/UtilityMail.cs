// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UtilityMail.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the UtilityMail type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.Tools.Utils
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Net.Mail;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    /// <summary>
    /// The utility mail.
    /// </summary>
    public class UtilityMail
    {
        /// <summary>
        /// The send message.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        /// <param name="body">
        /// The body.
        /// </param>
        /// <param name="replacements">
        /// The replacements.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool SendMessage(WebControl owner, string body, ListDictionary replacements, string to, string subject)
        {
            MailMessage mailMessage = CreateMessage(owner, body, replacements, to, null, null, subject,true);
            return InternalSend(mailMessage, false);
        }

        /// <summary>
        /// The send message.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        /// <param name="body">
        /// The body.
        /// </param>
        /// <param name="replacements">
        /// The replacements.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="htmlbody">
        /// The html body.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool SendMessage(WebControl owner, string body, ListDictionary replacements, string to, string subject, bool htmlbody)
        {
            MailMessage mailMessage = CreateMessage(owner, body, replacements, to, null, null, subject, htmlbody);
            return InternalSend(mailMessage, false);
        }

        /// <summary>
        /// The create message.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        /// <param name="body">
        /// The body.
        /// </param>
        /// <param name="replacements">
        /// The replacements.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="cc">
        /// The cc.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="htmlbody">
        /// The html body.
        /// </param>
        /// <returns>
        /// The <see cref="MailMessage"/>.
        /// </returns>
        private static MailMessage CreateMessage(Control owner, string body, ListDictionary replacements, string to, string from, string cc, string subject, bool htmlbody)
        {
            var md = new MailDefinition { Subject = subject, From = @from, CC = cc, IsBodyHtml = htmlbody };

            // this actually processes all of the replacement vars that you've included.
            var mailMessage = md.CreateMailMessage(to, replacements, body, owner);
            return mailMessage;
        }

        /// <summary>
        /// The internal send.
        /// </summary>
        /// <param name="mailMessage">
        /// The mail message.
        /// </param>
        /// <param name="isAsync">
        /// The is async.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool InternalSend(MailMessage mailMessage, bool isAsync)
        {
            try
            {
                if (!isAsync)
                {
                    // blocks current thread
                    SendMailMessage(mailMessage);
                }
                else
                {
                    // does not block current thread
                    SendMailMessageAsync(mailMessage);
                }
            }
            catch (Exception exc)
            {
                General.TraceWarn(exc.ToString());
                throw exc;
            }

            return true;
        }

        /// <summary>
        /// The send mail message.
        /// </summary>
        /// <param name="mailMessage">
        /// The mail message.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool SendMailMessage(MailMessage mailMessage)
        {
            try
            {
                General.TraceWrite("SendMailMessage() called");

                var client = new SmtpClient();

                // send copy to admin
                if (!string.IsNullOrEmpty(Config.GetConfigValue("adminEMail")))
                {
                    mailMessage.Bcc.Add(Config.GetConfigValue("adminEMail"));
                }

                General.TraceWrite("client.Port=" + client.Port.ToString());

                if (client.Port != 25)
                {
                    client.EnableSsl = true;
                }

                client.Send(mailMessage);
            }
            catch (Exception exc)
            {

                General.TraceWarn(exc.ToString());
                throw exc;
            }

            return true;
        }

        /// <summary>
        /// The send mail message async.
        /// </summary>
        /// <param name="mailMessage">
        /// The mail message.
        /// </param>
        /// <exception cref="Exception">
        /// </exception>
        public static void SendMailMessageAsync(MailMessage mailMessage)
        {
            try
            {
                General.TraceWrite("SendMailMessageAsync() called");

                var client = new SmtpClient();

                if (client.Port != 25)
                {
                    client.EnableSsl = true;
                }

                client.SendCompleted += ClientSendCompleted;
                client.SendAsync(mailMessage, mailMessage.To.ToString());
            }
            catch (Exception ex)
            {
                General.TraceWarn(ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// The client send completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public static void ClientSendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
        }

    }
}