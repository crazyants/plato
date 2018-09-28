using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace Plato.Internal.Layout.ModelBinding
{
    public class ControllerModelUpdater : IUpdateModel
    {

        private readonly Controller _controller;

        public ModelStateDictionary ModelState { get; }

        public RouteData RouteData { get; }

        public ViewDataDictionary ViewData { get; }

        public HttpContext HttpContext { get; }

        public ControllerModelUpdater(Controller controller)
        {
            _controller = controller;
        }
        
        public Task<bool> TryUpdateModelAsync<TModel>(TModel model) where TModel : class
        {
            return _controller.TryUpdateModelAsync<TModel>(model);
        }

        public bool TryValidateModel(object model)
        {
            return _controller.TryValidateModel(model);
        }

        public bool TryValidateModel(object model, string prefix)
        {
            return _controller.TryValidateModel(model, prefix);
        }

    }
}
