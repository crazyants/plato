using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Localization.Abstractions;

namespace Plato.Internal.Layout.TagHelpers
{

    [HtmlTargetElement("datetime")]
    public class DateTimeTagHelper : TagHelper
    {

        public DateTime? Value { get; set;  }

        private readonly IContextFacade _contextFacade;
        private readonly ITimeZoneProvider _timeZoneProvider;
    

        public DateTimeTagHelper(
            IContextFacade contextFacade, 
            ITimeZoneProvider timeZoneProvider)
        {
            _contextFacade = contextFacade;
            _timeZoneProvider = timeZoneProvider;
        }
        
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            if (this.Value == null)
            {
                throw new ArgumentNullException(nameof(this.Value));
            }
            
            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;

            var localDateTime = await GetLocalDateTime();
            var builder = new HtmlContentBuilder();

            var htmlContentBuilder = builder.AppendHtml(localDateTime.ToString(CultureInfo.InvariantCulture));
       
            output.Content.SetHtmlContent(htmlContentBuilder);
            
        }

        async Task<DateTime> GetLocalDateTime()
        {

            if (this.Value == null)
            {
                throw new ArgumentNullException(nameof(this.Value));
            }

            var date = (DateTime)this.Value;
            var serverTimeZoneId = "UTC";
            var userTimeZoneId = "UTC";
                
            var settings = await _contextFacade.GetSiteSettingsAsync();
            if (settings != null)
            {
                if (!String.IsNullOrEmpty(settings.TimeZone))
                {
                    serverTimeZoneId = settings.TimeZone;
                }
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user != null)
            {
                if (!String.IsNullOrEmpty(user.TimeZone))
                {
                    userTimeZoneId = user.TimeZone;
                }
            }

            // Get timezones taking DST into account

            TimeZoneInfo serverTimeZone;
            TimeZoneInfo userTimeZone;

            try
            {
                // Get specified time zone
                serverTimeZone = await _timeZoneProvider.GetTimeZoneByIdAsync(serverTimeZoneId);
                if (serverTimeZone != null)
                {
                    // Get timezone for adjusted daylight savings
                    if (serverTimeZone.SupportsDaylightSavingTime)
                    {
                        serverTimeZone = TimeZoneInfo.FindSystemTimeZoneById(serverTimeZone.IsDaylightSavingTime(date)
                            ? serverTimeZone.DaylightName
                            : serverTimeZone.StandardName);
                    }
                }
            }
            catch (TimeZoneNotFoundException)
            {
                throw new Exception($"The registry does not define the '{serverTimeZoneId}' time zone.");
            }
            catch (InvalidTimeZoneException)
            {
                throw new Exception($"Registry data on the '{serverTimeZoneId}' time zone has been corrupted.");
            }
            
            try
            {
                // Get specified time zone
                userTimeZone = await _timeZoneProvider.GetTimeZoneByIdAsync(userTimeZoneId);
                if (userTimeZone != null)
                {
                    if (userTimeZone.SupportsDaylightSavingTime)
                    {
                        // Get timezones for adjusted daylight savings
                        userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
                            userTimeZone.IsDaylightSavingTime(date)
                                ? userTimeZone.DaylightName
                                : userTimeZone.StandardName);
                    }
                }
            
            }
            catch (TimeZoneNotFoundException)
            {
                throw new Exception($"The registry does not define the {serverTimeZoneId}Time zone.");
            }
            catch (InvalidTimeZoneException)
            {
                throw new Exception($"Registry data on the '{serverTimeZoneId}' time zone has been corrupted.");
            }

            if (serverTimeZone == null)
            {
                throw new Exception($"A system timezone for the Id '{serverTimeZoneId}' could not be found");
            }

            if (userTimeZone == null)
            {
                throw new Exception($"A system timezone for the Id '{userTimeZoneId}' could not be found");
            }

            // Return local date time
            if (serverTimeZone.Id != userTimeZone.Id)
            {
                return TimeZoneInfo.ConvertTime(date, serverTimeZone, userTimeZone);
            }

            return date;

        }

    }
}
