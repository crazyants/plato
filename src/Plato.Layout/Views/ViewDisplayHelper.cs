using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Plato.Layout.ViewAdaptors;

namespace Plato.Layout.Views
{
    public interface IViewDisplayHelper
    {
        Task<IHtmlContent> DisplayAsync(IView view);
    }

    public class ViewDisplayHelper : IViewDisplayHelper
    {

        private readonly IViewAdaptorManager _viewAdaptorManager;
        private readonly IGenericViewInvoker _generaticViewInvoker;
        private readonly IGenericViewFactory _genericViewFactory;
        private readonly IServiceProvider _serviceProvider;

        public ViewContext ViewContext { get; set; }
        
        public ViewDisplayHelper(
            IGenericViewInvoker generaticViewInvoker,
            IGenericViewFactory genericViewFactory,
            ViewContext viewContext,
            IServiceProvider serviceProvider)
        {
            _generaticViewInvoker = generaticViewInvoker;
            _genericViewFactory = genericViewFactory;
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
            var viewDescriptor = await _genericViewFactory.CreateAsync(view);

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
            return await _genericViewFactory.InvokeAsync(displayContext);

        }

    }
}
