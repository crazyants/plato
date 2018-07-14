using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Plato.Internal.Assets.Abstractions;

namespace Plato.Internal.Layout.TagHelpers
{
    
    [HtmlTargetElement("script", Attributes = "section")]
    public class ScriptCaptureTagHelper : TagHelper
    {
  
        const string SectionAttributeName = "section";
        const string PriorityAttributeName = "priority";
        const string AllowMergeAttributeName = "allow-merge";

        private static readonly string[] SystemAttributes = new[] { SectionAttributeName, PriorityAttributeName, AllowMergeAttributeName };

        [HtmlAttributeName(SectionAttributeName)]
        public AssetSection Section { get; set; }

        [HtmlAttributeName(PriorityAttributeName)]
        public int? Priority { get; set; }

        [HtmlAttributeName(AllowMergeAttributeName)]
        public bool? AllowMerge { get; set; }
        
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            var attributes = context.AllAttributes
                .Where(a => !SystemAttributes.Contains(a.Name))
                .ToDictionary(k => k.Name, v => v.Value);
            var content = await output.GetChildContentAsync();

            var key = $"Script_{Section.ToString()}";
            ScriptCapture capture = null;
            if (ViewContext.HttpContext.Items.ContainsKey(key))
            {
                capture = ViewContext.HttpContext.Items[key] as ScriptCapture;
            }

            if (capture == null)
            {
                capture = new ScriptCapture();
                ViewContext.HttpContext.Items.Add(key, capture);
            }

            var order = Priority ?? int.MaxValue;
            capture.Add(content, attributes, order, AllowMerge);
            output.SuppressOutput();

        }

    }
}
