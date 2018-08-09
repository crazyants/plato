using System;

namespace Plato.Internal.Localization
{
    public class LocalDateTimeOptions
    {
        // Consts
        public const string DefaultTimeZone = "UTC";

        // Backing fields
        private string _serverTimeZone;
        private string _clientTimeZone;
        
        // Properties

        public DateTimeOffset UtcDateTime { get; set; }
        
        public string ServerTimeZone
        {
            get
            {
                if (!String.IsNullOrEmpty(_serverTimeZone))
                {
                    return _serverTimeZone;
                }

                return DefaultTimeZone;
            }
            set => _serverTimeZone = value;
        }

        public string ClientTimeZone
        {
            get
            {
                if (!String.IsNullOrEmpty(_clientTimeZone))
                {
                    return _clientTimeZone;
                }

                return DefaultTimeZone;
            }
            set => _clientTimeZone = value;
        }
        
        public bool ApplyClientTimeZoneOffset { get; set; }
        
    }

}
