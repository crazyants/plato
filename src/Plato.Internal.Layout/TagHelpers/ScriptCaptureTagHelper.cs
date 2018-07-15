using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Plato.Internal.Scripting.Abstractions;

namespace Plato.Internal.Layout.TagHelpers
{
    
    [HtmlTargetElement("script", Attributes = "section")]
    public class ScriptCaptureTagHelper : TagHelper
    {
  
        const string SectionAttributeName = "section";
        const string PriorityAttributeName = "priority";
        const string AllowMergeAttributeName = "allow-merge";
        const string EnsureUniqueAttributeName = "ensure-unique";

        private static readonly string[] SystemAttributes = new[]
        {
            SectionAttributeName,
            PriorityAttributeName,
            AllowMergeAttributeName,
            EnsureUniqueAttributeName
        };

        [HtmlAttributeName(SectionAttributeName)]
        public ScriptSection Section { get; set; }

        [HtmlAttributeName(PriorityAttributeName)]
        public int? Priority { get; set; }

        [HtmlAttributeName(AllowMergeAttributeName)]
        public bool AllowMerge { get; set; }

        [HtmlAttributeName(EnsureUniqueAttributeName)]
        public bool EnsureUnique { get; set; }
        
        private readonly IScriptManager _scriptManager;

        public ScriptCaptureTagHelper(IScriptManager scriptManager)
        {
            _scriptManager = scriptManager;
        }
        
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            var attributes = context.AllAttributes
                .Where(a => !SystemAttributes.Contains(a.Name))
                .ToDictionary(k => k.Name, v => v.Value);

            var content = await output.GetChildContentAsync();

            var order = Priority ?? int.MaxValue;
            
            _scriptManager.RegisterScriptBlock(
                new ScriptBlock(content, attributes, order, AllowMerge, EnsureUnique),
                Section);

            output.SuppressOutput();

        }

    }
}
