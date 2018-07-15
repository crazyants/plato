using System;
using System.Collections.Generic;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Internal.Scripting.Abstractions
{
    public class ScriptBlocks
    {

        readonly IList<ScriptBlock> _blocks = new List<ScriptBlock>();

        public IEnumerable<ScriptBlock> Blocks => _blocks;
        
        public void Add(ScriptBlock block)
        {

            if (block.EnsureUnique)
            {
                // Ensure script blocks we register are unique
                var exists = false;
                foreach (var localBlock in _blocks)
                {
                    if (localBlock.Content.Stringify().Equals(block.Content.Stringify(), StringComparison.OrdinalIgnoreCase))
                    {
                        exists = true;
                        break;
                    }
                }

                if (exists)
                {
                    return;
                }

            }

            lock (_blocks)
            {
                _blocks.Add(block);
            }

        }
       
    }


}
