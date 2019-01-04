using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace Plato.Internal.Layout.Views
{
    public interface IViewFactory
    {
        ViewDescriptor Create(IView view);

        Task<IHtmlContent> InvokeAsync(ViewDisplayContext displayContext);

    }

}
