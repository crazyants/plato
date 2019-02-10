using Microsoft.AspNetCore.Html;
using System.Collections.Generic;

namespace Plato.Internal.Scripting.Abstractions
{

    public class ScriptBlock
    {

        public ScriptBlock(string script)
            : this(new HtmlString(script))
        {

        }

        public ScriptBlock(string script, int order)
            : this(new HtmlString(script), null, order)
        {

        }


        public ScriptBlock(IHtmlContent content)
           : this(content, null)
        {
        }

        public ScriptBlock(IHtmlContent content, Dictionary<string, object> attributes)
            : this(content, attributes, int.MaxValue)
        {
        }

        public ScriptBlock(Dictionary<string, object> attributes)
            : this(null, attributes, int.MaxValue)
        {
        }
        public ScriptBlock(Dictionary<string, object> attributes, int order)
            : this(null, attributes, order)
        {
        }

        public ScriptBlock(IHtmlContent content, Dictionary<string, object> attributes, int order)
            : this(content, attributes, order, true, true)
        {
        }

        public ScriptBlock(
            IHtmlContent content,
            Dictionary<string, object> attributes,
            int order,
            bool canMerge,
            bool ensureUnique)
        {
            Content = content;
            Attributes = attributes;
            Order = order;
            CanMerge = canMerge;
            EnsureUnique = ensureUnique;
        }

        public IHtmlContent Content { get; }

        public int Order { get; }

        public Dictionary<string, object> Attributes { get; }

        public bool CanMerge { get; set; }
        
        public bool EnsureUnique { get; set; }

    }
    
    public enum ScriptSection
    {
        Header,
        Body,
        Footer
    }

}
