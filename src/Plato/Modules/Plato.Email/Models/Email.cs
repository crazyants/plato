using System;
using System.Data;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models;

namespace Plato.Email.Models
{
    public class Email : IModel<Email>
    {
        
        public int Id { get; set; }

        public string To { get; set; }

        public string Cc { get; set; }

        public string Bcc { get; set; }

        public string From { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public EmailPriority Priority { get; set; }

        public short Attempts { get; set; }

        public System.DateTime DateStamp { get; set; }


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

            if (dr.ColumnIsNotNull("Attempts"))
                Attempts = Convert.ToInt16(dr["Attempts"]);

            if (dr.ColumnIsNotNull("DateStamp"))
                DateStamp = Convert.ToDateTime(dr["DateStamp"]);
            
        }

    }

    public enum EmailPriority
    {
        High = 1,
        Medium = 2,
        Low = 3
    }

}
