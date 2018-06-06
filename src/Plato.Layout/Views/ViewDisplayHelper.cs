using System;
using System.Collections.Generic;
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
using Plato.Layout.Adaptors;

namespace Plato.Layout.Views
{
    public interface IViewDisplayHelper
    {
        Task<IHtmlContent> DisplayAsync(IGenericView view);
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
        
        public async Task<IHtmlContent> DisplayAsync(IGenericView view)
        {

            if (view == null)
            {
                return HtmlString.Empty;
            }
            
            // Build view descriptor
            var viewDescriptor = await _genericViewFactory.CreateAsync(view.Name, view);

            // Get registered view adaptor providers
            var viewAdaptorManager = ViewContext.HttpContext.RequestServices.GetService<IViewAdaptorManager>();
            var adaptors = await viewAdaptorManager.GetViewAdaptors(view.Name);

            // Build display context
            var displayContext = new GenericViewDisplayContext()
            {
                ViewDescriptor = viewDescriptor,
                ViewAdaptors = adaptors,
                ViewContext = this.ViewContext,
                ServiceProvider = _serviceProvider
            };

            // Invoke the view
            return await _genericViewFactory.InvokeAsync(displayContext);

        }

    }
}
