using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Localization.Abstractions;

namespace Plato.Internal.Localization
{

    public class LocalDateTimeProvider : ILocalDateTimeProvider
    {
        
        private readonly ITimeZoneProvider _timeZoneProvider;
        private readonly ILogger<LocalDateTimeProvider> _logger;

        public LocalDateTimeProvider(
            ITimeZoneProvider timeZoneProvider,
            ILogger<LocalDateTimeProvider> logger)
        {
            _timeZoneProvider = timeZoneProvider;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<DateTimeOffset> GetLocalDateTimeAsync(LocalDateTimeOptions options)
        {

            // We always need options
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            
            // We always need a UTC date to localize
            if (options.UtcDateTime == null)
            {
                throw new ArgumentNullException(nameof(options.UtcDateTime));
            }

            // UTC supplied date to convert
            var utcDateTimeOffset = options.UtcDateTime;
            
            // Get time zones 
            var serverTimeZoneInfo = await GetTimeZoneInfoAsync(options.ServerTimeZone);
            var userTimeZoneInfo = await GetTimeZoneInfoAsync(options.ClientTimeZone);

            // Get server UTC offset
            var serverOffset = serverTimeZoneInfo.GetUtcOffset(utcDateTimeOffset);

            try
            {

                // Apply offset for client
                if (options.ApplyClientTimeZoneOffset)
                {
                    // Get UTC offset for client
                    var userOffset = userTimeZoneInfo.GetUtcOffset(utcDateTimeOffset);
                    // Return local date / time
                    return utcDateTimeOffset.ToOffset(userOffset);
                }

                // Apply offset for server
                return utcDateTimeOffset.ToOffset(serverOffset); ;

            }
            catch (Exception ex)
            {
                // swallow but log exceptions
                _logger.LogCritical(
                    $"An error occurred applying the offset for '{utcDateTimeOffset.DateTime}' with the server time zones '{options.ServerTimeZone}' and client time zone '{options.ClientTimeZone}' - {ex.Message}");
            }

            return utcDateTimeOffset;

        }
        
        #endregion

        #region "Private Methods"
        
        async Task<TimeZoneInfo> GetTimeZoneInfoAsync(string id)
        {

            // We always need a timezone so we know the offsets to perform the conversion
            var timeZoneInfo = await _timeZoneProvider.GetTimeZoneByIdAsync(id);
            if (timeZoneInfo == null)
            {
                throw new Exception($"A system time zone for the Id '{id}' could not be found");
            }

            return timeZoneInfo;

        }

        #endregion

    }

}
