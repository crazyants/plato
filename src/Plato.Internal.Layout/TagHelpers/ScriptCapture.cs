using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Plato.Internal.Layout.TagHelpers
{

    public class ScriptCapture
    {

        private readonly List<ScriptBlock> _scriptBlocks = new List<ScriptBlock>();

        public void Add(TagHelperContent content, Dictionary<string, object> attributes, int order, bool? canMerge = null)
        {
            var block = new ScriptBlock(content, attributes, order, canMerge);
            lock (_scriptBlocks)
            {
                _scriptBlocks.Add(block);
            }
        }

        public IEnumerable<ScriptBlock> Blocks => _scriptBlocks;

        public struct ScriptBlock
        {

            public ScriptBlock(TagHelperContent content, Dictionary<string, object> attributes, int order, bool? canMerge)
            {
                Content = content;
                Attributes = attributes;
                Order = order;
                CanMerge = canMerge;
            }

            public TagHelperContent Content { get; }

            public int Order { get; }

            public bool? CanMerge { get; set; }

            public Dictionary<string, object> Attributes { get; }
        }

    }


}
