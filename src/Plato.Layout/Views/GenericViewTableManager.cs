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
        Task<GenericViewDescriptor> TryAdd(string key, object model);
    }

    public class GenericViewTableManager : IGenericViewTableManager
    {

      

        private static readonly ConcurrentDictionary<string, GenericViewDescriptor> _views =
            new ConcurrentDictionary<string, GenericViewDescriptor>();

        public async Task<GenericViewDescriptor> TryAdd(string name, object value)
        {

          
            var descriptor = new GenericViewDescriptor()
            {
                Name = name,
                Value = value
            };

            _views.TryAdd(name, descriptor);

            return descriptor;

        }


    }

}
