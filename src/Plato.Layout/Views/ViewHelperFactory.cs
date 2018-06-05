using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Plato.Layout.Views
{

    public interface IViewHelperFactory
    {
        IViewHelper CreateHelper(ViewContext context);
    }

    public class ViewHelperFactory : IViewHelperFactory
    {

        private readonly IHtmlHelper _htmlHelper;
        private readonly IHtmlDisplay _displayManager;
        private readonly IViewResultFactory _viewResultFactory;
        private readonly IServiceProvider _serviceProvider;
        
        public ViewHelperFactory(
            IHtmlHelper htmlHelper,
            IHtmlDisplay displayManager,
            IViewResultFactory viewResultFactory,
            IServiceProvider serviceProvider)
        {
            _htmlHelper = htmlHelper;
            _displayManager = displayManager;
            _viewResultFactory = viewResultFactory;
            _serviceProvider = serviceProvider;
        }

        public IViewHelper CreateHelper(ViewContext viewContext)
        {
            return new ViewHelper(
                _htmlHelper,
                _displayManager,
                _viewResultFactory,
                viewContext,
                _serviceProvider);
        }
    }
}
