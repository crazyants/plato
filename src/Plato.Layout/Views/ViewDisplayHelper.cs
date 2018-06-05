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

        private readonly IHtmlHelper _htmlHelper;
        private readonly IViewComponentHelper _viewComponentHelper;
        private readonly IHtmlDisplay _htmlDisplay;
        private readonly IViewResultFactory _shapeFactory;
        private readonly IServiceProvider _serviceProvider;

        public ViewContext ViewContext { get; set; }
        
        public ViewDisplayHelper(
            IHtmlHelper htmlHelper,
            IViewComponentHelper viewComponentHelper,
            IHtmlDisplay htmlDisplay,
            IViewResultFactory viewResultFactory,
            ViewContext viewContext,
            IServiceProvider serviceProvider)
        {
            _htmlHelper = htmlHelper;
            _viewComponentHelper = viewComponentHelper;
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

            // view components use an anonymous type for the parameters argument
            // this anonymous type is emitted as an actual type by the compiler but
            // marked with the CompilerGeneratedAttribute. If we find this attribute
            // on the model we'll treat this view as a ViewComponent and invoke accordingly
            if (IsViewModelAnonymousType(view))
            {

                // view component
                var helper = _viewComponentHelper as DefaultViewComponentHelper;
                if (helper == null)
                {
                    throw new ArgumentNullException(
                        $"{_viewComponentHelper.GetType()} cannot be converted to DefaultViewComponentHelper");
                }

                helper.Contextualize(this.ViewContext);
                return await _viewComponentHelper.InvokeAsync(view.Name, view.Model);

            }
            else
            {

                // partial view

                var helper = _htmlHelper as HtmlHelper;
                if (helper == null)
                {
                    throw new ArgumentNullException($"{_htmlHelper.GetType()} cannot be converted to HtmlHelper");
                }
                helper.Contextualize(this.ViewContext);
                return await _htmlHelper.PartialAsync(view.Name, view.Model, ViewContext.ViewData);

            }
            
            return HtmlString.Empty;

        }

        public bool IsViewModelAnonymousType(IGenericView view)
        {
            return view.Model
                .GetType()
                .GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Any();
        }
    }
}
