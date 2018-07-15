using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Plato.Internal.Scripting.Abstractions;

namespace Plato.Internal.Layout.TagHelpers
{

    [HtmlTargetElement("scripts", Attributes = "section")]
    public class ScriptRenderTagHelper : TagHelper
    {
        private const string ScriptTag = "script";

        [HtmlAttributeName("section")]
        public ScriptSection Section { get; set; }

        [HtmlAttributeName("auto-merge")]
        public bool AutoMerge { get; set; }
        
        private readonly IScriptManager _scriptManager;

        public ScriptRenderTagHelper(IScriptManager scriptManager)
        {
            _scriptManager = scriptManager;
        }
        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            var capture = _scriptManager.GetScriptBlocks(this.Section);
            if (capture == null)
            {
                return;
            }

            var result = new HelperResult(async tw => await RenderBlocks(tw, capture));

            output.TagName = null;
            output.Content.SetHtmlContent(result);
        }

        async Task RenderBlocks(TextWriter tw, ScriptBlocks blocks)
        {

            var orderedBlocks = blocks.Blocks.OrderBy(b => b.Order);
            var orderedBlocksList = orderedBlocks.ToList();

            var mergableBlocks = orderedBlocksList.Where(b =>
                ((AutoMerge && (!b.CanMerge.HasValue || b.CanMerge.Value)) ||
                (!AutoMerge && b.CanMerge.HasValue && b.CanMerge.Value)));
            
            var otherBlocks = orderedBlocksList.Except(mergableBlocks);

            var mergableBlocksList = mergableBlocks.ToList();

            await RenderMergedBlocks(tw, mergableBlocksList);

            await RenderSeparateBlocks(tw, otherBlocks);

        }

        async Task RenderSeparateBlocks(TextWriter tw, IEnumerable<ScriptBlock> blocks)
        {
            foreach (var block in blocks)
            {
                var tagBuilder = new TagBuilder(ScriptTag)
                {
                    TagRenderMode = TagRenderMode.Normal
                };
                tagBuilder.InnerHtml.AppendHtml(block.Content);
                tagBuilder.MergeAttributes(block.Attributes, replaceExisting: true);
                tagBuilder.WriteTo(tw, NullHtmlEncoder.Default);

                await tw.WriteLineAsync();

            }

        }

        async Task RenderMergedBlocks(TextWriter tw, IEnumerable<ScriptBlock> blocks)
        {

            var blockList = blocks.ToList();

            if (!blockList.Any())
                return;

            var tagBuilder = new TagBuilder(ScriptTag)
            {
                TagRenderMode = TagRenderMode.Normal
            };

            foreach (var block in blockList)
            {
                tagBuilder.InnerHtml.AppendHtml(block.Content);
                tagBuilder.MergeAttributes(block.Attributes, replaceExisting: true);
            }

            tagBuilder.WriteTo(tw, NullHtmlEncoder.Default);
            await tw.WriteLineAsync();

        }

    }

}
