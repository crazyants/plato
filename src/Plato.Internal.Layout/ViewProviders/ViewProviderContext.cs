using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Layout.ModelBinding;

namespace Plato.Internal.Layout.ViewProviders
{

    public interface IViewProviderContext : IUpdateModel
    {

        RouteData RouteData { get; }

        ViewDataDictionary ViewData { get; }

        HttpContext HttpContext { get; }
    }

    public class ViewProviderContext :
        ControllerModelUpdater, IViewProviderContext
    {
        public RouteData RouteData { get; }

        public ViewDataDictionary ViewData { get; }

        public HttpContext HttpContext { get; }

        public ViewProviderContext(Controller controller) : base(controller)
        {

        }
    }
}
