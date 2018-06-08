using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Plato.Layout.ModelBinding
{
    public class ControllerModelUpdater : IUpdateModel
    {

        private readonly Controller _controller;

        public ModelStateDictionary ModelState { get; }

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
