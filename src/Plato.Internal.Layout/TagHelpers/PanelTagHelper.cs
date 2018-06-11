using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Plato.Internal.Layout.TagHelpers
{

    [HtmlTargetElement("panel")]
    public class PanelTagHelper : TagHelper
    {

        [ViewContext] // inform razor to inject
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            var content = await output.GetChildContentAsync();

            output.TagName = "div";
            output.Attributes.Add("class", "panel");

            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetHtmlContent(content);

        }


    }
}
