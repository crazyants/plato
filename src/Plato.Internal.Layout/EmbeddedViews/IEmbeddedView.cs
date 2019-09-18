using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Plato.Internal.Layout.EmbeddedViews
{
    public interface IEmbeddedView
    {

        IEmbeddedView Contextualize(ViewContext context);

        Task<IHtmlContent> Build();

    }

}
