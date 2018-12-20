using Plato.Internal.Text.Diff.DiffBuilder.Model;

namespace Plato.Internal.Text.Diff.DiffBuilder
{
    public interface IInlineDiffBuilder
    {
        DiffPaneModel BuildDiffModel(string oldText, string newText);
    }
}
