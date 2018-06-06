using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plato.Layout.Views;

namespace Plato.Layout.Adaptors
{

    public interface IViewAdaptorManager
    {
        Task<IEnumerable<IViewAdaptorResult>> GetAdaptos(string name);

    }

    public class ViewAdaptorManager : IViewAdaptorManager
    {
        private readonly IList<IViewAdaptorProvider> _adaptorProviders;

        private static ConcurrentDictionary<string, IViewAdaptorResult> _adaptors;


        public ViewAdaptorManager(
            IEnumerable<IViewAdaptorProvider> adaptorProviders)
        {
            _adaptorProviders = adaptorProviders.ToList();
        }

        public async Task<IEnumerable<IViewAdaptorResult>> GetAdaptos(string name)
        {

            await ConfigureAdaptors();

            return _adaptors?.Values;

        }
        
        public async Task ConfigureAdaptors()
        {

            if (_adaptors == null)
            {
                if (_adaptorProviders.Count > 0)
                {
                    _adaptors = new ConcurrentDictionary<string, IViewAdaptorResult>();
                    foreach (var adaptor in _adaptorProviders)
                    {
                        try
                        {
                            var result = await adaptor.ConfigureAsync();
                            _adaptors.TryAdd(result.ViewName, result);
                        }
                        catch (Exception e)
                        {
                            //_logger.LogError(e, $"An exception occurred while building the menu: {name}");
                        }
                    }
                }

            }
            
        }

    }
}
