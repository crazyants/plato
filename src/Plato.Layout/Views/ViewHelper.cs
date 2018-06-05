using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Plato.Layout.Views
{
    public interface IViewHelper
    {
        Task<IHtmlContent> DisplayAsync(View view);
    }

    public class ViewHelper : DynamicObject, IViewHelper
    {

        private readonly IHtmlHelper _htmlHelper;
        private readonly IHtmlDisplay _htmlDisplay;
        private readonly IViewResultFactory _shapeFactory;
        private readonly IServiceProvider _serviceProvider;

        public ViewContext ViewContext { get; set; }
        
        public ViewHelper(
            IHtmlHelper htmlHelper,
            IHtmlDisplay htmlDisplay,
            IViewResultFactory viewResultFactory,
            ViewContext viewContext,
            IServiceProvider serviceProvider)
        {
            _htmlHelper = htmlHelper;
            _htmlDisplay = htmlDisplay;
            _shapeFactory = viewResultFactory;
            ViewContext = viewContext;
            _serviceProvider = serviceProvider;
        }
        
        public async Task<IHtmlContent> DisplayAsync(View view)
        {

            if (view == null)
            {
                return HtmlString.Empty;
            }


            var helper = _htmlHelper as HtmlHelper;

            if (helper == null)
            {
                throw new ArgumentNullException($"{_htmlHelper.GetType()} cannot be converted to HtmlHelper");
            }

            helper.Contextualize(this.ViewContext);
            return await _htmlHelper.PartialAsync(view.Name, view.Model, ViewContext.ViewData);
            
        }
        
    }
}
