using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Plato.Layout.ViewAdaptors;

namespace Plato.Layout.Views
{

    public interface IGenericViewInvoker
    {
        ViewContext ViewContext { get; set; }

        void Contextualize(ViewDisplayContext viewContext);

        Task<IHtmlContent> InvokeAsync(string viewName, object model);

    }

    public class ViewInvoker : IGenericViewInvoker
    {

        public ViewContext ViewContext { get; set; }

        private readonly IHtmlHelper _htmlHelper;
        private readonly IViewComponentHelper _viewComponentHelper;
        private readonly ILogger<ViewInvoker> _logger;

        public ViewInvoker(
            IHtmlHelper htmlHelper,
            IViewComponentHelper viewComponentHelper,
            ILogger<ViewInvoker> logger)
        {
            _htmlHelper = htmlHelper;
            _viewComponentHelper = viewComponentHelper;
            _logger = logger;
        }

        // implementation

        public void Contextualize(ViewDisplayContext context)
        {
            this.ViewContext = context.ViewContext;
        }

        public async Task<IHtmlContent> InvokeAsync(string viewName, object model)
        {
            if (this.ViewContext == null)
            {
                throw new Exception(
                    "ViewContext must be set via the Contextualize method before calling the InvokeAsync method");
            }


            // view components use an anonymous type for the parameters argument
            // this anonymous type is emitted as an actual type by the compiler but
            // marked with the CompilerGeneratedAttribute. If we find this attribute
            // on the model we'll treat this view as a ViewComponent and invoke accordingly
            if (IsViewModelAnonymousType(model))
            {
                return await InvokeViewComponentAsync(viewName, model);
            }

            // else we have a partial view
            return await InvokePartialAsync(viewName, model);


        }

        // privates

        async Task<IHtmlContent> InvokePartialAsync(string viewName, object model)
        {
            var helper = _htmlHelper as HtmlHelper;
            if (helper == null)
            {
                throw new ArgumentNullException($"{_htmlHelper.GetType()} cannot be converted to HtmlHelper");
            }
            helper.Contextualize(this.ViewContext);
            return await _htmlHelper.PartialAsync(viewName, model, ViewContext.ViewData);
        }

        async Task<IHtmlContent> InvokeViewComponentAsync(string viewName, object model)
        {
            var helper = _viewComponentHelper as DefaultViewComponentHelper;
            if (helper == null)
            {
                throw new ArgumentNullException(
                    $"{_viewComponentHelper.GetType()} cannot be converted to DefaultViewComponentHelper");
            }
            helper.Contextualize(this.ViewContext);
            return await _viewComponentHelper.InvokeAsync(viewName, model);
        }
        
        bool IsViewModelAnonymousType(object model)
        {
            return model
                .GetType()
                .GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Any();
        }

    }
}
