using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Plato.Internal.Layout.Views
{
    public interface IViewInvoker
    {
        ViewContext ViewContext { get; set; }

        void Contextualize(ViewDisplayContext viewContext);

        Task<IHtmlContent> InvokeAsync(IView view);

    }
}
