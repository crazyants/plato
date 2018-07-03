using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Plato.Internal.Layout.TagHelpers
{
    [HtmlTargetElement(Attributes = "asp-autofocus")]
    public class AutoFocusTagHelper  : TagHelper
    {

        [HtmlAttributeName("asp-autofocus")]
        public bool AutoFocus { get; set; }
        
        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            if (AutoFocus)
            {
                output.Attributes.SetAttribute("autofocus", "");
            }
            
            return Task.CompletedTask;
        }

    }
}
