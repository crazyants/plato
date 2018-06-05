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

namespace Plato.Layout.Views
{
    public interface IViewDisplayHelper
    {
        Task<IHtmlContent> DisplayAsync(IGenericView view);
    }

    public class ViewDisplayHelper : IViewDisplayHelper
    { 

        private readonly IGenericViewInvoker _generaticViewInvoker;

        private readonly IHtmlDisplay _htmlDisplay;
        private readonly IGenericViewFactory _genericViewFactory;
        private readonly IServiceProvider _serviceProvider;

        public ViewContext ViewContext { get; set; }
        
        public ViewDisplayHelper(
            IGenericViewInvoker generaticViewInvoker,
            IHtmlDisplay htmlDisplay,
            IGenericViewFactory genericViewFactory,
            ViewContext viewContext,
            IServiceProvider serviceProvider)
        {
            _generaticViewInvoker = generaticViewInvoker;
            _htmlDisplay = htmlDisplay;
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
            
            var viewDescriptor = await _genericViewFactory.CreateAsync(view.Name, view);
            

            var displayContext = new GenericViewDisplayContext()
            {
                ViewDescriptor = viewDescriptor,
                ViewContext = this.ViewContext
            };

            var output = await _genericViewFactory.InvokeAsync(displayContext);

            //_generaticViewInvoker.Contextualize(this.ViewContext);
            //var output = await _generaticViewInvoker.InvokeAsync(view);
            
            return output;

        }

    }
}
