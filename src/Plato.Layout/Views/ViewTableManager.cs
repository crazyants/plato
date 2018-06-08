using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Layout.ViewAdaptors;

namespace Plato.Layout.Views
{

    public interface IGenericViewTableManager
    {
        Task<ViewDescriptor> TryAdd(IView view);
    }

    public class ViewTableManager : IGenericViewTableManager
    {

      

        private static readonly ConcurrentDictionary<string, ViewDescriptor> _views =
            new ConcurrentDictionary<string, ViewDescriptor>();

        public async Task<ViewDescriptor> TryAdd(IView view)
        {
            
            var descriptor = new ViewDescriptor()
            {
                Name = view.ViewName,
                View = view
            };

            _views.TryAdd(view.ViewName, descriptor);

            return descriptor;

        }


    }

}
