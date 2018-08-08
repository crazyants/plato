using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Internal.Layout.TagHelpers
{

    // https://docs.microsoft.com/en-us/dotnet/standard/datetime/converting-between-time-zones

    [HtmlTargetElement("datetime")]
    public class DateTimeTagHelper : TagHelper
    {

        public const string UtcId = "UTC";

        public DateTime? Value { get; set;  }

        public bool EnablePrettyDate { get; set; } = true;

        private readonly IContextFacade _contextFacade;
        private readonly ITimeZoneProvider _timeZoneProvider;
        private readonly ILogger<DateTimeTagHelper> _logger;

        public DateTimeTagHelper(
            IContextFacade contextFacade, 
            ITimeZoneProvider timeZoneProvider,
            ILogger<DateTimeTagHelper> logger)
        {
            _contextFacade = contextFacade;
            _timeZoneProvider = timeZoneProvider;
            _logger = logger;
        }
        
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;
            
            var builder = new HtmlContentBuilder();
            var htmlContentBuilder = builder.AppendHtml(await GetDisplayValue());
       
            output.Content.SetHtmlContent(htmlContentBuilder);
            
        }

        async Task<string> GetDisplayValue()
        {

            if (this.Value == null) return "-";

            var localDateTime = await GetLocalDateTime();

            return this.EnablePrettyDate
                ? localDateTime.Date.ToPrettyDate()
                : localDateTime.ToString(CultureInfo.InvariantCulture);

        }

        async Task<DateTimeOffset> GetLocalDateTime()
        {

            if (this.Value == null)
            {
                throw new ArgumentNullException(nameof(this.Value));
            }
            
            // Supplied date to convert
            var date = (DateTime)this.Value;

            // Client & server
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            var settings = await _contextFacade.GetSiteSettingsAsync();

            // Default client & server to UTC
            string serverTimeZone = GetServerTimeZone(settings), 
                clientTimeZone = GetClientTimeZone(user);
                
            // Get timezones 
            var serverTimeZoneInfo = await GetTimeZoneInfoAsync(serverTimeZone);
            var  userTimeZoneInfo = await GetTimeZoneInfoAsync(clientTimeZone);
          
            // Get UTC offsets
            var offset = new DateTimeOffset(date);
            var serverOffset = serverTimeZoneInfo.GetUtcOffset(offset);
            var userOffset = userTimeZoneInfo.GetUtcOffset(offset);
          
            try
            {

                // Convert date to servers offset
                var serverTime = offset.ToOffset(serverOffset);

                // If user is authenticated, convert
                // servers offset to users local offset
                if (user != null)
                {
                    return serverTime.ToOffset(userOffset);
                }

                return serverTime;

            }
            catch (FormatException)
            {
                // Sink exceptions but log
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogCritical($"A error occurred applying the offset for '{date}' with the following time zones '{serverTimeZone}' and '{clientTimeZone}'.");
                }
            }

            return date;

        }


        async Task<TimeZoneInfo> GetTimeZoneInfoAsync(string id)
        {
            var timeZoneInfo = await _timeZoneProvider.GetTimeZoneByIdAsync(id);
            if (timeZoneInfo == null)
            {
                throw new Exception($"A system time zone for the Id '{id}' could not be found");
            }

            return timeZoneInfo;

        }
        string GetServerTimeZone(ISiteSettings settings)
        {
            if (settings != null)
            {
                if (!String.IsNullOrEmpty(settings.TimeZone))
                {
                    return settings.TimeZone;
                }
            }
            return UtcId;
        }

        string GetClientTimeZone(User user)
        {
            if (user != null)
            {
                if (!String.IsNullOrEmpty(user.TimeZone))
                {
                    return user.TimeZone;
                }
            }
            return UtcId;
        }
        
    }
}
