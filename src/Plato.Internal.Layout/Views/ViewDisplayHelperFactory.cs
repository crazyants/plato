using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Plato.Internal.Layout.Views
{

    public interface IViewHelperFactory
    {
        IViewDisplayHelper CreateHelper(ViewContext context);
    }

    public class ViewDisplayHelperFactory : IViewHelperFactory
    {

        readonly IViewInvoker _viewInvoker;
        readonly IViewFactory _viewFactory;
        readonly IServiceProvider _serviceProvider;
        
        public ViewDisplayHelperFactory(
            IViewInvoker viewInvoker,
            IViewFactory viewFactory,
            IServiceProvider serviceProvider)
        {
            _viewInvoker = viewInvoker;
            _viewFactory = viewFactory;
            _serviceProvider = serviceProvider;
        }

        public IViewDisplayHelper CreateHelper(ViewContext viewContext)
        {
            return new ViewDisplayHelper(
                _viewInvoker,
                _viewFactory,
                viewContext,
                _serviceProvider);
        }
    }
}
