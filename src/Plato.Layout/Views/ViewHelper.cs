using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Plato.Layout.Views
{
    public interface IViewHelper
    {
        Task<IHtmlContent> DisplayAsync(View view);
    }

    public class ViewHelper : DynamicObject, IViewHelper
    {

        private readonly IHtmlDisplay _htmlDisplay;
        private readonly IViewResultFactory _shapeFactory;
        private readonly IServiceProvider _serviceProvider;

        public ViewContext ViewContext { get; set; }

        public ViewHelper(
            IHtmlDisplay htmlDisplay,
            IViewResultFactory viewResultFactory,
            ViewContext viewContext,
            IServiceProvider serviceProvider)
        {
            _htmlDisplay = htmlDisplay;
            _shapeFactory = viewResultFactory;
            ViewContext = viewContext;
            _serviceProvider = serviceProvider;
        }
        
        public Task<IHtmlContent> DisplayAsync(View view)
        {

            if (view == null)
            {
                return Task.FromResult<IHtmlContent>(HtmlString.Empty);
            }

            throw new NotImplementedException();

        }
        
    }
}
