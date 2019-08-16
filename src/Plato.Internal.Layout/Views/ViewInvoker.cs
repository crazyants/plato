﻿using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;

namespace Plato.Internal.Layout.Views
{

    public class ViewInvoker : IViewInvoker
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

        // Implementation

        public void Contextualize(ViewDisplayContext context)
        {
            this.ViewContext = context.ViewContext;
        }

        public async Task<IHtmlContent> InvokeAsync(IView view)
        {
            if (this.ViewContext == null)
            {
                throw new Exception(
                    "ViewContext must be set via the Contextualize method before calling the InvokeAsync method");
            }

            // Embedded views simply return the output generated within the view
            // It's the embedded views responsibility to perform model binding
            // Embedded views can leverage the current context within the Build method
            if (view.EmbeddedView != null)
            {
                
                return await view.EmbeddedView
                    .Contextualize(this.ViewContext)
                    .Build();
            }
            
            // View components use an anonymous type for the parameters argument
            // this anonymous type is emitted as an actual type by the compiler but
            // marked with the CompilerGeneratedAttribute. If we find this attribute
            // on the model we'll treat this view as a ViewComponent and invoke accordingly
            if (IsViewModelAnonymousType(view.Model))
            {
                return await InvokeViewComponentAsync(view.ViewName, view.Model);
            }

            // else we have a partial view
            return await InvokePartialAsync(view.ViewName, view.Model);
            
        }

        // privates

        async Task<IHtmlContent> InvokePartialAsync(string viewName, object model)
        {
            if (!(_htmlHelper is HtmlHelper helper))
            {
                throw new ArgumentNullException($"{_htmlHelper.GetType()} cannot be converted to HtmlHelper");
            }
            helper.Contextualize(this.ViewContext);

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Attempting to invoke partial view \"{viewName}\".");
            }

            try
            {
                return await _htmlHelper.PartialAsync(viewName, model, ViewContext.ViewData);
            }
            catch (Exception e)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(e,
                        $"An exception occurred whilst invoking the partial view with name \"{viewName}\". {e.Message}");
                }
                throw;
            }
            

        }

        async Task<IHtmlContent> InvokeViewComponentAsync(string viewName, object arguments)
        {
            if (!(_viewComponentHelper is DefaultViewComponentHelper helper))
            {
                throw new ArgumentNullException(
                    $"{_viewComponentHelper.GetType()} cannot be converted to DefaultViewComponentHelper");
            }

            // Contextualize view component
            helper.Contextualize(this.ViewContext);

            // Log the invocation, we can't use try / catch around our view component helper :(
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Attempting to invoke view component \"{viewName}\".");
            }

            try
            {
                return await _viewComponentHelper.InvokeAsync(viewName, arguments);
            }
            catch (Exception e)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(e,
                        $"An exception occurred whilst invoking the view component with name \"{viewName}\". {e.Message}");
                }
                throw;
            }
            
            
        }
        
        bool IsViewModelAnonymousType(object model)
        {

            // We need a model to inspect
            if (model == null)
            {
                return false;
            }

            return model
                .GetType()
                .GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Any();

        }

    }
}
