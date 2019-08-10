using Plato.Internal.Abstractions;

namespace Plato.Internal.Emails.Abstractions
{
    public class EmailSettings : Serializable
    {

        public Pop3Settings Pop3Settings { get; set; }

        public SmtpSettings SmtpSettings { get; set; }

    }

}
