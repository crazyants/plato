using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Plato.Internal.Assets.Abstractions;

namespace Plato.Internal.Layout.TagHelpers
{
    [HtmlTargetElement("scripts", Attributes = "section")]
    public class ScriptRenderTagHelper : TagHelper
    {
        private const string ScriptTag = "script";

        [HtmlAttributeName("section")]
        public AssetSection Section { get; set; }

        [HtmlAttributeName("auto-merge")]
        public bool AutoMerge { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            var key = $"Script_{Section.ToString()}";
            if (!ViewContext.HttpContext.Items.ContainsKey(key) ||
                !(ViewContext.HttpContext.Items[key] is ScriptCapture capture))
                return;

            output.TagName = null;
            output.Content.SetHtmlContent(new HelperResult(async tw => await RenderBlocks(tw, capture)));
        }

        private async Task RenderBlocks(TextWriter tw, ScriptCapture capture)
        {
            var orderedBlocks = capture.Blocks.OrderBy(b => b.Order);
            var orderedBlocksList = orderedBlocks.ToList();
            var mergableBlocks = orderedBlocksList.Where(b =>
                ((AutoMerge && (!b.CanMerge.HasValue || b.CanMerge.Value)) ||
                (!AutoMerge && b.CanMerge.HasValue && b.CanMerge.Value))
                && !b.Content.IsEmptyOrWhiteSpace);
            var otherBlocks = orderedBlocksList.Except(mergableBlocks);

            var mergableBlocksList = mergableBlocks.ToList();

            await RenderMergedBlocks(tw, mergableBlocksList);
            await RenderSeparateBlocks(tw, otherBlocks);
        }

        private async Task RenderSeparateBlocks(TextWriter tw, IEnumerable<ScriptCapture.ScriptBlock> blocks)
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

        private async Task RenderMergedBlocks(TextWriter tw, IEnumerable<ScriptCapture.ScriptBlock> blocks)
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
