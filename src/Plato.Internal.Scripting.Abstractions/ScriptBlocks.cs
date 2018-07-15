using System.Collections.Generic;
using Microsoft.AspNetCore.Html;

namespace Plato.Internal.Scripting.Abstractions
{
    public class ScriptBlocks
    {

        private readonly IList<ScriptBlock> _blocks = new List<ScriptBlock>();

        public void Add(IHtmlContent content, Dictionary<string, object> attributes, int order, bool? canMerge = null)
        {
            var block = new ScriptBlock(content, attributes, order, canMerge);
            lock (_blocks)
            {
                _blocks.Add(block);
            }
        }

        public IEnumerable<ScriptBlock> Blocks => _blocks;
        
    }


}
