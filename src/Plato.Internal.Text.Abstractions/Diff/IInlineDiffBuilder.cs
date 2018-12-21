using Plato.Internal.Text.Abstractions.Diff.Models;

namespace Plato.Internal.Text.Abstractions.Diff
{
    public interface IInlineDiffBuilder
    {
        DiffPaneModel BuildDiffModel(string oldText, string newText);
    }
}
