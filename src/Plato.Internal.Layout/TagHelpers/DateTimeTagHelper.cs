using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Plato.Internal.Hosting.Abstractions;

namespace Plato.Internal.Layout.TagHelpers
{

    //[HtmlTargetElement("datetime")]
    //public class DateTimeTagHelper : TagHelper
    //{

    //    public DateTime? Value { get; set;  }

    //    private readonly IContextFacade _contextFacade;
    //    //private readonly ITimeZoneProvider _timeZoneProvider;
    

    //    public DateTimeTagHelper(
    //        IContextFacade contextFacade)
    //    {
    //        _contextFacade = contextFacade;
    //    }
        
    //    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    //    {

    //        output.TagName = "span";
    //        output.TagMode = TagMode.StartTagAndEndTag;
            
    //        var builder = new HtmlContentBuilder();
    //        output.Content.SetHtmlContent(await ConvertDateTime(builder));
            
    //    }

    //    async Task<IHtmlContent> ConvertDateTime(HtmlContentBuilder builder)
    //    {

    //        var serverTimeZone = "";
    //        var usertimeZone = "";
                

    //        var settings = await _contextFacade.GetSiteSettingsAsync();
    //        if (settings == null)
    //        {

    //        }

    //        var user = await _contextFacade.GetAuthenticatedUserAsync();
    //        if (user == null)
    //        {

    //        }

    //        var htmlContentBuilder = builder.AppendHtml("datetime");
    //        return htmlContentBuilder;

    //    }

    //}
}
