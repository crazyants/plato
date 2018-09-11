using System;
using System.Data;
using System.Net.Mail;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models;

namespace Plato.Internal.Emails.Abstractions
{
    public class EmailMessage : IModel<EmailMessage>
    {
        
        public int Id { get; set; }

        public string To { get; set; }

        public string Cc { get; set; }

        public string Bcc { get; set; }

        public string From { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public EmailPriority Priority { get; set; }

        public short SendAttempts { get; set; }

        public int CreatedUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public EmailMessage()
        {

        }

        public EmailMessage(MailMessage message)
        {

            if (message.To.Count > 0)
            {
                this.To = message.To[0].Address;
            }

            if (message.CC.Count > 0)
            {
                this.Cc = message.CC[0].Address;
            }

            if (message.Bcc.Count > 0)
            {
                this.Bcc = message.Bcc[0].Address;
            }

            if (message.From != null)
            {
                this.From = message.From.Address;
            }
            
            this.Subject = message.Subject;
            this.Body = message.Body;


        }

        public void PopulateModel(IDataReader dr)
        {


            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("To"))
                To = Convert.ToString(dr["To"]);

            if (dr.ColumnIsNotNull("Cc"))
                Cc = Convert.ToString(dr["Cc"]);

            if (dr.ColumnIsNotNull("Bcc"))
                Bcc = Convert.ToString(dr["Bcc"]);

            if (dr.ColumnIsNotNull("From"))
                From = Convert.ToString(dr["From"]);

            if (dr.ColumnIsNotNull("Subject"))
                Subject = Convert.ToString(dr["Subject"]);

            if (dr.ColumnIsNotNull("Body"))
                Body = Convert.ToString(dr["Body"]);

            if (dr.ColumnIsNotNull("Priority"))
                Priority = (EmailPriority)Convert.ToInt32(dr["Priority"]);

            if (dr.ColumnIsNotNull("SendAttempts"))
                SendAttempts = Convert.ToInt16(dr["SendAttempts"]);

            if (dr.ColumnIsNotNull("CreatedUserId"))
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);
            
            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);
            
        }

    }

    public enum EmailPriority
    {
        High = 1,
        Medium = 2,
        Low = 3
    }

}
