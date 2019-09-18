using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Layout.ViewAdapters;

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

        private IViewAdapterManager _viewAdapterManager;

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
            //var viewDescriptor = _viewFactory.Create(view);

            // Get registered view adapter providers for the view
            if (_viewAdapterManager == null)
            {
                _viewAdapterManager = ViewContext.HttpContext.RequestServices.GetService<IViewAdapterManager>();
            }
            
            var viewAdapterResults = await _viewAdapterManager.GetViewAdaptersAsync(view.ViewName);

            // Invoke the view with supplied context
            return await _viewFactory.InvokeAsync(new ViewDisplayContext()
            {
                ViewDescriptor = new ViewDescriptor()
                {
                    Name = view.ViewName,
                    View = view
                },
                ViewAdaptorResults = viewAdapterResults,
                ViewContext = ViewContext,
                ServiceProvider = _serviceProvider
            });

        }

    }

}
