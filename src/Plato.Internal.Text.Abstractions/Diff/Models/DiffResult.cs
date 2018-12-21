using System.Collections.Generic;

namespace Plato.Internal.Text.Abstractions.Diff.Models
{
 
    public class DiffResult
    {

        public string[] PiecesOld { get; }
        
        public string[] PiecesNew { get; }
        
        public IList<DiffBlock> DiffBlocks { get; }

        public DiffResult(string[] peicesOld, string[] piecesNew, IList<DiffBlock> blocks)
        {
            PiecesOld = peicesOld;
            PiecesNew = piecesNew;
            DiffBlocks = blocks;
        }
    }
}