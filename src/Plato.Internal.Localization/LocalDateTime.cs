using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Plato.Internal.Localization.Abstractions;

namespace Plato.Internal.Localization
{
    

    public class LocalDateTime : ILocalDateTime
    {
        
        // DI
        private readonly ITimeZoneProvider _timeZoneProvider;
        private readonly ILogger<LocalDateTime> _logger;

        public LocalDateTime(
            ITimeZoneProvider timeZoneProvider,
            ILogger<LocalDateTime> logger,
            IHttpContextAccessor httpContextAccessor)
        {
      
            _timeZoneProvider = timeZoneProvider;
            _logger = logger;
        }


        #region "Implementation"

        public async Task<DateTimeOffset> GetLocalDateTimeAsync(LocalDateTimeOptions options)
        {

            if (options.UtcDateTime == null)
            {
                throw new ArgumentNullException(nameof(options.UtcDateTime));
            }

            // Supplied date to convert
            var date = (DateTime)options.UtcDateTime;

            // Default client & server to UTC
            string serverTimeZone = GetServerTimeZone(options),
                clientTimeZone = GetClientTimeZone(options);

            // Get timezones 
            var serverTimeZoneInfo = await GetTimeZoneInfoAsync(serverTimeZone);
            var userTimeZoneInfo = await GetTimeZoneInfoAsync(clientTimeZone);

            // Get UTC offsets
            var offset = new DateTimeOffset(date);
            var serverOffset = serverTimeZoneInfo.GetUtcOffset(offset);

            try
            {

                // Convert date to servers offset
                var serverTime = offset.ToOffset(serverOffset);

                // convert servers offset to clients local offset
                if (options.ApplyClientTimeZoneOffset)
                {
                    var userOffset = userTimeZoneInfo.GetUtcOffset(offset);
                    return serverTime.ToOffset(userOffset);
                }

                return serverTime;

            }
            catch (FormatException)
            {
                // Sink exceptions but log
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogCritical($"A error occurred applying the offset for '{date}' with the server time zones '{serverTimeZone}' and client time zone '{clientTimeZone}'.");
                }
            }

            return date;

        }


        #endregion

        #region "Private Methods"


        async Task<TimeZoneInfo> GetTimeZoneInfoAsync(string id)
        {
            var timeZoneInfo = await _timeZoneProvider.GetTimeZoneByIdAsync(id);
            if (timeZoneInfo == null)
            {
                throw new Exception($"A system time zone for the Id '{id}' could not be found");
            }

            return timeZoneInfo;

        }

        string GetServerTimeZone(LocalDateTimeOptions options)
        {
            if (options != null)
            {
                if (!String.IsNullOrEmpty(options.ServerTimeZone))
                {
                    return options.ServerTimeZone;
                }
            }
            return LocalDateTimeOptions.DefaultTimeZone;
        }


        string GetClientTimeZone(LocalDateTimeOptions options)
        {
            if (options != null)
            {
                if (!String.IsNullOrEmpty(options.ClientTimeZone))
                {
                    return options.ClientTimeZone;
                }
            }
            return LocalDateTimeOptions.DefaultTimeZone;
        }

        #endregion

    }
}
