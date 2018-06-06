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
        private readonly IGenericViewFactory _genericViewFactory;
        private readonly IServiceProvider _serviceProvider;
        
        public ViewDisplayHelperFactory(
            IGenericViewInvoker genericViewInvoker,
            IGenericViewFactory genericViewFactory,
            IServiceProvider serviceProvider)
        {
            _genericViewInvoker = genericViewInvoker;
            _genericViewFactory = genericViewFactory;
            _serviceProvider = serviceProvider;
        }

        public IViewDisplayHelper CreateHelper(ViewContext viewContext)
        {
            return new ViewDisplayHelper(
                _genericViewInvoker,
                _genericViewFactory,
                viewContext,
                _serviceProvider);
        }
    }
}
