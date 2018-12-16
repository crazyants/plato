using System.Threading.Tasks;
using Plato.Internal.Layout.ViewAdaptors;

namespace Plato.Markdown.ViewAdaptors
{
    public class EditorViewAdaptorProvider : BaseAdaptorProvider
    {

        public override Task<IViewAdaptorResult> ConfigureAsync()
        {
            return Adapt("Editor", v =>
            {
                v.AdaptView("Markdown");
            });
        }

    }

}
