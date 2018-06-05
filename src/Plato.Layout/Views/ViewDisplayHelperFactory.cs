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
        IViewDisplayHelper CreateHelper(ViewContext context);
    }

    public class ViewDisplayHelperFactory : IViewHelperFactory
    {

        private readonly IGenericViewInvoker _genericViewInvoker;
        private readonly IHtmlDisplay _displayManager;
        private readonly IGenericViewFactory _genericViewFactory;
        private readonly IServiceProvider _serviceProvider;
        
        public ViewDisplayHelperFactory(
            IGenericViewInvoker genericViewInvoker,
            IHtmlDisplay displayManager,
            IGenericViewFactory genericViewFactory,
            IServiceProvider serviceProvider)
        {
            _genericViewInvoker = genericViewInvoker;
            _displayManager = displayManager;
            _genericViewFactory = genericViewFactory;
            _serviceProvider = serviceProvider;
        }

        public IViewDisplayHelper CreateHelper(ViewContext viewContext)
        {
            return new ViewDisplayHelper(
                _genericViewInvoker,
                _displayManager,
                _genericViewFactory,
                viewContext,
                _serviceProvider);
        }
    }
}
