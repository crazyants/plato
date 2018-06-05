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
    public interface IViewHelper
    {
        Task<IHtmlContent> DisplayAsync(IGenericView view);
    }

    public class ViewDisplayHelper : DynamicObject, IViewHelper
    {



        private readonly IGenericViewInvoker _generaticViewInvoker;

        private readonly IHtmlDisplay _htmlDisplay;
        private readonly IViewResultFactory _shapeFactory;
        private readonly IServiceProvider _serviceProvider;

        public ViewContext ViewContext { get; set; }
        
        public ViewDisplayHelper(
            IGenericViewInvoker generaticViewInvoker,
            IHtmlDisplay htmlDisplay,
            IViewResultFactory viewResultFactory,
            ViewContext viewContext,
            IServiceProvider serviceProvider)
        {
            _generaticViewInvoker = generaticViewInvoker;
            _htmlDisplay = htmlDisplay;
            _shapeFactory = viewResultFactory;
            ViewContext = viewContext;
            _serviceProvider = serviceProvider;
        }
        
        public async Task<IHtmlContent> DisplayAsync(IGenericView view)
        {

            if (view == null)
            {
                return HtmlString.Empty;
            }

            _generaticViewInvoker.Contextualize(this.ViewContext);
            var output = await _generaticViewInvoker.InvokeAsync(view);
            
            return output;

        }

    }
}
