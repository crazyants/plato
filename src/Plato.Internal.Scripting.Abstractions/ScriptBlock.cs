using Microsoft.AspNetCore.Html;
using System.Collections.Generic;

namespace Plato.Internal.Scripting.Abstractions
{

    public class ScriptBlock
    {

        public ScriptBlock(IHtmlContent content)
           : this(content, null)
        {
        }

        public ScriptBlock(IHtmlContent content, Dictionary<string, object> attributes)
            : this(content, attributes, int.MaxValue)
        {
        }

        public ScriptBlock(IHtmlContent content, Dictionary<string, object> attributes, int order)
            : this(content, attributes, int.MaxValue, true)
        {
        }

        public ScriptBlock(IHtmlContent content, Dictionary<string, object> attributes, int order, bool? canMerge)
        {
            Content = content;
            Attributes = attributes;
            Order = order;
            CanMerge = canMerge;
        }

        public IHtmlContent Content { get; }

        public int Order { get; }

        public bool? CanMerge { get; set; }

        public Dictionary<string, object> Attributes { get; }
    }
    
    public enum ScriptSection
    {
        Header,
        Body,
        Footer
    }

}
