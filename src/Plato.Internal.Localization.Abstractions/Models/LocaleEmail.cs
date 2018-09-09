namespace Plato.Internal.Localization.Abstractions.Models
{

    public class LocaleEmail : ILocaleValue
    {

        public string Key { get; set; }

        public string To { get; set; }

        public string From { get; set; }

        public string Cc { get; set; }

        public string Bcc { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

    }

}
