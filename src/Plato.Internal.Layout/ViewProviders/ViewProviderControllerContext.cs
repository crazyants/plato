using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Layout.ModelBinding;

namespace Plato.Internal.Layout.ViewProviders
{

    public interface IViewProviderControllerContext
    {

        RouteData RouteData { get; }

        ViewDataDictionary ViewData { get; }

        HttpContext HttpContext { get; }
    }

    public class ViewProviderControllerContext :
        ControllerModelUpdater, IViewProviderControllerContext
    {
        public RouteData RouteData { get; }

        public ViewDataDictionary ViewData { get; }

        public HttpContext HttpContext { get; }

        public ViewProviderControllerContext(Controller controller) : base(controller)
        {
        }
    }
}
