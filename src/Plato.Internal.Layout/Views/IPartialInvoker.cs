using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Plato.Internal.Layout.Views
{
    public interface IPartialInvoker
    {

        ViewContext ViewContext { get; set; }

        void Contextualize(ViewContext viewContext);

        Task<IHtmlContent> InvokeAsync(string viewName, object model, ViewDataDictionary viewData);

    }

}
