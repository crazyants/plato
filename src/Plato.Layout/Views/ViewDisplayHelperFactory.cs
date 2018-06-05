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

        private readonly IGenericViewInvoker _genericViewInvoker;
        private readonly IHtmlDisplay _displayManager;
        private readonly IViewResultFactory _viewResultFactory;
        private readonly IServiceProvider _serviceProvider;
        
        public ViewDisplayHelperFactory(
            IGenericViewInvoker genericViewInvoker,
            IHtmlDisplay displayManager,
            IViewResultFactory viewResultFactory,
            IServiceProvider serviceProvider)
        {
            _genericViewInvoker = genericViewInvoker;
            _displayManager = displayManager;
            _viewResultFactory = viewResultFactory;
            _serviceProvider = serviceProvider;
        }

        public IViewHelper CreateHelper(ViewContext viewContext)
        {
            return new ViewDisplayHelper(
                _genericViewInvoker,
                _displayManager,
                _viewResultFactory,
                viewContext,
                _serviceProvider);
        }
    }
}
