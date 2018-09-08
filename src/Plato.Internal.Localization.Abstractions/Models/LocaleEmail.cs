using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Internal.Localization.Abstractions.Models
{

    public class LocaleEmails
    {
        public IEnumerable<LocaleEmail> Templates { get; set; } = new List<LocaleEmail>();

    }

    public class LocaleEmail
    {

        public string Id { get; set; }

        public string To { get; set; }

        public string From { get; set; }

        public string Cc { get; set; }

        public string Bcc { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

    }


}
