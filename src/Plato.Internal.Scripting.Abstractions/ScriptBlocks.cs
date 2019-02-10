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
                    
                    // Is the content unique?
                    if (block.Content != null && localBlock.Content != null)
                    {
                        if (localBlock.Content.Stringify().Equals(block.Content.Stringify(), StringComparison.OrdinalIgnoreCase))
                        {
                            exists = true;
                            break;
                        }
                    }

                    // Are the attributes unique?
                    if (block.Attributes != null && localBlock.Attributes != null)
                    {
                        var identicalAttributes = true;
                        foreach (var localAttribute in localBlock.Attributes)
                        {
                            foreach (var blockAttribute in block.Attributes)
                            {
                                if (blockAttribute.Key == localAttribute.Key)
                                {
                                    if (blockAttribute.Value != localAttribute.Value)
                                    {
                                        identicalAttributes = false;
                                    }
                                }
                            }
                        }

                        if (identicalAttributes)
                        {
                            exists = true;
                            break;
                        }
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
