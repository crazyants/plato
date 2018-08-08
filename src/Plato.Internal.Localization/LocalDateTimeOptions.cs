using System;

namespace Plato.Internal.Localization
{
    public class LocalDateTimeOptions
    {
        // Defaults
        public const string DefaultTimeZone = "UTC";

        public DateTime? UtcDateTime { get; set; }

        public string ServerTimeZone { get; set; } = DefaultTimeZone;

        public string ClientTimeZone { get; set; } = DefaultTimeZone;
        
        public bool ApplyClientTimeZoneOffset { get; set; }
        
    }

}
