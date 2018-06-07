using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Layout.Adaptors;

namespace Plato.Layout.Views
{

    public interface IGenericViewTableManager
    {
        Task<GenericViewDescriptor> TryAdd(IGenericView view);
    }

    public class GenericViewTableManager : IGenericViewTableManager
    {

      

        private static readonly ConcurrentDictionary<string, GenericViewDescriptor> _views =
            new ConcurrentDictionary<string, GenericViewDescriptor>();

        public async Task<GenericViewDescriptor> TryAdd(IGenericView view)
        {
            
            var descriptor = new GenericViewDescriptor()
            {
                Name = view.ViewName,
                View = view
            };

            _views.TryAdd(view.ViewName, descriptor);

            return descriptor;

        }


    }

}
