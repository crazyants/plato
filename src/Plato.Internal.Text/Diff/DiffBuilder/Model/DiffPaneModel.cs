using System.Collections.Generic;

namespace Plato.Internal.Text.Diff.DiffBuilder.Model
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