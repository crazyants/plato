using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Layout.ViewAdaptors;

namespace Plato.Internal.Layout.Views
{
    public interface IViewDisplayHelper
    {
        Task<IHtmlContent> DisplayAsync(IView view);
    }

    public class ViewDisplayHelper : IViewDisplayHelper
    {
        
        private readonly IViewFactory _viewFactory;
        private readonly IServiceProvider _serviceProvider;

        public ViewContext ViewContext { get; set; }
        
        public ViewDisplayHelper(
            IViewInvoker viewInvoker,
            IViewFactory viewFactory,
            ViewContext viewContext,
            IServiceProvider serviceProvider)
        {
            _viewFactory = viewFactory;
            ViewContext = viewContext;
            _serviceProvider = serviceProvider;
        }
        
        public async Task<IHtmlContent> DisplayAsync(IView view)
        {

            if (view == null)
            {
                return HtmlString.Empty;
            }
            
            // Build view descriptor
            var viewDescriptor = await _viewFactory.CreateAsync(view);

            // Get registered view adaptor providers for the view
            var viewAdaptorManager = ViewContext.HttpContext.RequestServices.GetService<IViewAdaptorManager>();
            var viewAdaptorResults = await viewAdaptorManager.GetViewAdaptors(view.ViewName);

            // Build display context
            var displayContext = new ViewDisplayContext()
            {
                ViewDescriptor = viewDescriptor,
                ViewAdaptorResults = viewAdaptorResults,
                ViewContext = this.ViewContext,
                ServiceProvider = _serviceProvider
            };

            // Invoke the view
            return await _viewFactory.InvokeAsync(displayContext);

        }

    }
}
