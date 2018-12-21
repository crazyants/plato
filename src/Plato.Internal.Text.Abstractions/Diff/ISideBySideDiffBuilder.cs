using Plato.Internal.Text.Abstractions.Diff.Models;

namespace Plato.Internal.Text.Abstractions.Diff
{

    public interface ISideBySideDiffBuilder
    {
        SideBySideDiffModel BuildDiffModel(string oldText, string newText);
    }
}