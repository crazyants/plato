using System.Collections.Generic;

namespace Plato.Internal.Text.Abstractions.Diff.Models
{
    public class DiffPaneModel
    {
        public List<DiffPiece> Lines { get; }

        public DiffPaneModel()
        {
            Lines = new List<DiffPiece>();
        }
    }
}