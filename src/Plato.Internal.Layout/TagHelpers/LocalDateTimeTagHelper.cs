using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Localization.Abstractions;

namespace Plato.Internal.Layout.TagHelpers
{

    [HtmlTargetElement("date")]
    public class LocalDateTimeTagHelper : TagHelper
    {

        // Defaults
        public const string DefaultDateTimeFormat = "G";

        // Properties

        [HtmlAttributeName("utc")]
        public DateTimeOffset? Utc { get; set;  }

        [HtmlAttributeName("pretty")]
        public bool Pretty { get; set; } = true;
        
        private readonly ILocalDateTimeProvider _localDateTimeProvider;
        private readonly IContextFacade _contextFacade;

        private readonly SiteOptions _settings;

        public LocalDateTimeTagHelper(
            ILocalDateTimeProvider localDateTimeProvider,
            IOptions<SiteOptions> siteSettings,
            IContextFacade contextFacade)
        {
            _localDateTimeProvider = localDateTimeProvider;
            _contextFacade = contextFacade;
            _settings = siteSettings.Value;
        }
        
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            // Client & server
            var utcDateTime = Utc ?? DateTimeOffset.UtcNow;
            var user = await _contextFacade.GetAuthenticatedUserAsync();
         
            // Get local date time, only apply client offset if user is authenticated
            var localDateTime = await _localDateTimeProvider.GetLocalDateTimeAsync(new LocalDateTimeOptions()
            {
                UtcDateTime = utcDateTime,
                ServerTimeZone = _settings?.TimeZone,
                ClientTimeZone = user?.TimeZone,
                ApplyClientTimeZoneOffset = user != null
            });

            // Date format
            var dateTimeFormat = GetDefaultDateTimeFormat();

            // We always need the full regular date for the title attribute (not the pretty date)
            var formattedDateTime = localDateTime.DateTime.ToString(dateTimeFormat);
            
            // Build output

            var builder = new HtmlContentBuilder();

            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("title", formattedDateTime);
            output.Attributes.Add("data-toggle", "tooltip");

            output.Content.SetHtmlContent(this.Pretty
                ? builder.AppendHtml(utcDateTime.DateTime.ToPrettyDate(dateTimeFormat))
                : builder.AppendHtml(formattedDateTime));

        }
        
        string GetDefaultDateTimeFormat()
        {
            if (_settings != null)
            {
                if (!String.IsNullOrEmpty(_settings.DateTimeFormat))
                {
                    return _settings.DateTimeFormat;
                }
            }
            return DefaultDateTimeFormat;
        }
        
    }
}
