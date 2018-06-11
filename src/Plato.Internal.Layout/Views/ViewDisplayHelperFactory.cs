using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Plato.Internal.Layout.ViewAdaptors;

namespace Plato.Internal.Layout.Views
{

    public interface IViewHelperFactory
    {
        IViewDisplayHelper CreateHelper(ViewContext context);
    }

    public class ViewDisplayHelperFactory : IViewHelperFactory
    {

        private readonly IViewAdaptorManager _viewAdaptorManager;
        private readonly IViewInvoker _genericViewInvoker;
        private readonly IGenericViewFactory _genericViewFactory;
        private readonly IServiceProvider _serviceProvider;
        
        public ViewDisplayHelperFactory(
            IViewInvoker genericViewInvoker,
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
