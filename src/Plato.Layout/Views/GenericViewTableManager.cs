using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Plato.Layout.Views
{

    public interface IGenericViewTableManager
    {
        Task<GenericViewDescriptor> TryGetOrAdd(string key, object model);
    }

    public class GenericViewTableManager : IGenericViewTableManager
    {

        private static readonly ConcurrentDictionary<string, GenericViewDescriptor> _views =
            new ConcurrentDictionary<string, GenericViewDescriptor>();

        public Task<GenericViewDescriptor> TryGetOrAdd(string name, object value)
        {

            var descriptor = new GenericViewDescriptor()
            {
                Name = name,
                Value = value
            };

            _views.TryAdd(name, descriptor);

            return Task.FromResult(descriptor);

            //if (!_views.ContainsKey(name))
            //{
            //    var descriptor = new GenericViewDescriptor()
            //    {
            //        Name = name,
            //        Value = value
            //    };
            //    _views[name] = descriptor;
            //}

            //return _views[name].Value;


        }


    }

}
