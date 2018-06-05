using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Plato.Layout.Views
{

    public interface IViewHelperFactory
    {
        IViewHelper CreateHelper(ViewContext context);
    }

    public class ViewDisplayHelperFactory : IViewHelperFactory
    {

        private readonly IHtmlHelper _htmlHelper;
        private readonly IViewComponentHelper _viewComponentHelper;
        private readonly IHtmlDisplay _displayManager;
        private readonly IViewResultFactory _viewResultFactory;
        private readonly IServiceProvider _serviceProvider;
        
        public ViewDisplayHelperFactory(
            IHtmlHelper htmlHelper,
            IViewComponentHelper viewComponentHelper,
            IHtmlDisplay displayManager,
            IViewResultFactory viewResultFactory,
            IServiceProvider serviceProvider)
        {
            _htmlHelper = htmlHelper;
            _viewComponentHelper = viewComponentHelper;
            _displayManager = displayManager;
            _viewResultFactory = viewResultFactory;
            _serviceProvider = serviceProvider;
        }

        public IViewHelper CreateHelper(ViewContext viewContext)
        {
            return new ViewDisplayHelper(
                _htmlHelper,
                _viewComponentHelper,
                _displayManager,
                _viewResultFactory,
                viewContext,
                _serviceProvider);
        }
    }
}
