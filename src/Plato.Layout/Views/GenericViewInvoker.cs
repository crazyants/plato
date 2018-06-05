using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;

namespace Plato.Layout.Views
{

    public interface IGenericViewInvoker
    {
        ViewContext ViewContext { get; set; }

        void Contextualize(GenericViewDisplayContext viewContext);

        Task<IHtmlContent> InvokeAsync(GenericViewDescriptor view);

    }

    public class GenericViewInvoker : IGenericViewInvoker
    {

        private readonly IHtmlHelper _htmlHelper;
        private readonly IViewComponentHelper _viewComponentHelper;
        private readonly ILogger<GenericViewInvoker> _logger;

        public ViewContext ViewContext { get; set; }

        public GenericViewInvoker(
            IHtmlHelper htmlHelper,
            IViewComponentHelper viewComponentHelper,
            ILogger<GenericViewInvoker> logger)
        {
            _htmlHelper = htmlHelper;
            _viewComponentHelper = viewComponentHelper;
            _logger = logger;
        }

        // implementation

        public void Contextualize(GenericViewDisplayContext context)
        {
            this.ViewContext = context.ViewContext;
        }

        public async Task<IHtmlContent> InvokeAsync(GenericViewDescriptor view)
        {
            if (this.ViewContext == null)
            {
                throw new Exception("ViewContext must be set via the Contextualize method before calling the InvokeAsync method");
            }

            // are we displaying a generic view?
            if (view.Value is IGenericView genericView)
            {
                // view components use an anonymous type for the parameters argument
                // this anonymous type is emitted as an actual type by the compiler but
                // marked with the CompilerGeneratedAttribute. If we find this attribute
                // on the model we'll treat this view as a ViewComponent and invoke accordingly
                if (IsViewModelAnonymousType(genericView))
                {
                    return await InvokeViewComponentAsync(genericView);
                }

                return await InvokePartialAsync(genericView);

            }


            return HtmlString.Empty;

        }
        
        // privates

        async Task<IHtmlContent> InvokePartialAsync(IGenericView view)
        {
            var helper = _htmlHelper as HtmlHelper;
            if (helper == null)
            {
                throw new ArgumentNullException($"{_htmlHelper.GetType()} cannot be converted to HtmlHelper");
            }
            helper.Contextualize(this.ViewContext);
            return await _htmlHelper.PartialAsync(view.Name, view.Model, ViewContext.ViewData);
        }

        async Task<IHtmlContent> InvokeViewComponentAsync(IGenericView view)
        {
            var helper = _viewComponentHelper as DefaultViewComponentHelper;
            if (helper == null)
            {
                throw new ArgumentNullException(
                    $"{_viewComponentHelper.GetType()} cannot be converted to DefaultViewComponentHelper");
            }
            helper.Contextualize(this.ViewContext);
            return await _viewComponentHelper.InvokeAsync(view.Name, view.Model);
        }
        
        bool IsViewModelAnonymousType(IGenericView view)
        {
            return view.Model
                .GetType()
                .GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Any();
        }

    }
}
