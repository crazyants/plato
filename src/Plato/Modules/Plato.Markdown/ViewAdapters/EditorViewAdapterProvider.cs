using System;
using System.Threading.Tasks;
using Plato.Internal.Layout.ViewAdapters;

namespace Plato.Markdown.ViewAdapters
{
    public class EditorViewAdapterProvider : BaseAdapterProvider
    {
            
        public EditorViewAdapterProvider()
        {
            ViewName = "Editor";
        }

        public override Task<IViewAdapterResult> ConfigureAsync(string viewName)
        {

            if (!viewName.Equals(ViewName, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(default(IViewAdapterResult));
            }
            
            return Adapt(ViewName, v =>
            {
                v.AdaptView("Markdown");
            });
        }

    }

}
