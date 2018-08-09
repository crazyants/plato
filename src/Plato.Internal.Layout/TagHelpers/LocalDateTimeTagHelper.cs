using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Localization;

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

        private readonly IContextFacade _contextFacade;
        private readonly ILocalDateTimeProvider _localDateTimeProvider;
        private readonly ILogger<LocalDateTimeTagHelper> _logger;

        public LocalDateTimeTagHelper(
            IContextFacade contextFacade, 
            ILogger<LocalDateTimeTagHelper> logger,
            ILocalDateTimeProvider localDateTimeProvider)
        {
            _contextFacade = contextFacade;
            _logger = logger;
            _localDateTimeProvider = localDateTimeProvider;
        }
        
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            // Client & server
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            var settings = await _contextFacade.GetSiteSettingsAsync();

            // Get local date time, only apply client offset if user is authenticated
            var localDateTime = await _localDateTimeProvider.GetLocalDateTimeAsync(new LocalDateTimeOptions()
            {
                UtcDateTime = Utc ?? DateTimeOffset.UtcNow,
                ServerTimeZone = settings?.TimeZone,
                ClientTimeZone = user?.TimeZone,
                ApplyClientTimeZoneOffset = user != null
            });

            // We always need the full regular date for the title attribute (not the pretty date)
            var formattedDateTime = localDateTime.DateTime.ToString(GetDefaultDateTimeFormat(settings));
            
            // Build output

            var builder = new HtmlContentBuilder();

            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("title", formattedDateTime);
          
            output.Content.SetHtmlContent(this.Pretty
                ? builder.AppendHtml(localDateTime.DateTime.ToPrettyDate())
                : builder.AppendHtml(formattedDateTime));

        }
        
        string GetDefaultDateTimeFormat(ISiteSettings settings)
        {
            if (settings != null)
            {
                if (!String.IsNullOrEmpty(settings.DateTimeFormat))
                {
                    return settings.DateTimeFormat;
                }
            }
            return DefaultDateTimeFormat;
        }
        
    }
}
