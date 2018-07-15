using System.Collections.Generic;

namespace Plato.Internal.Scripting.Abstractions
{
    public class ScriptBlocks
    {

        readonly IList<ScriptBlock> _blocks = new List<ScriptBlock>();

        public IEnumerable<ScriptBlock> Blocks => _blocks;
        
        public void Add(ScriptBlock block)
        {
            lock (_blocks)
            {
                _blocks.Add(block);
            }
        }
       
    }


}
