using Plato.Internal.Abstractions;

namespace Plato.Email.Models
{
    public class EmailSettings : Serializable
    {

        public Pop3Settings Pop3Settings { get; set; }

        public SmtpSettings SmtpSettings { get; set; }

    }

}
