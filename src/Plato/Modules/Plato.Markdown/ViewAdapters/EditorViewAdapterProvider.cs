using System.Threading.Tasks;
using Plato.Internal.Layout.ViewAdapters;

namespace Plato.Markdown.ViewAdapters
{
    public class EditorViewAdapterProvider : BaseAdapterProvider
    {

        public override Task<IViewAdapterResult> ConfigureAsync()
        {
            return Adapt("Editor", v =>
            {
                v.AdaptView("Markdown");
            });
        }

    }

}
