using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace Plato.Internal.Layout.Views
{
    public interface IViewFactory
    {
        Task<ViewDescriptor> CreateAsync(IView view);

        Task<IHtmlContent> InvokeAsync(ViewDisplayContext displayContext);

    }

}
