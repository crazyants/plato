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
        Task<IEnumerable<IViewAdaptorResult>> GetViewAdaptors(string name);

    }

    public class ViewAdaptorManager : IViewAdaptorManager
    {
        private readonly IList<IViewAdaptorProvider> _adaptorProviders;

        private static ConcurrentDictionary<string, IViewAdaptorResult> _adaptors
            = new ConcurrentDictionary<string, IViewAdaptorResult>();
        
        public ViewAdaptorManager(
            IEnumerable<IViewAdaptorProvider> adaptorProviders)
        {
            _adaptorProviders = adaptorProviders.ToList();
        }

        public async Task<IEnumerable<IViewAdaptorResult>> GetViewAdaptors(string viewName)
        {

            await ConfigureAdaptors();

            var matchingAdapatorResults = new List<IViewAdaptorResult>();
            foreach (var viewAdaptorResult in _adaptors)
            {
                if (viewAdaptorResult.Key.Equals(viewName))
                {
                    matchingAdapatorResults.Add(viewAdaptorResult.Value);
                }
            }

            return matchingAdapatorResults;
            
        }
        
        public async Task ConfigureAdaptors()
        {

            if (_adaptors.Count == 0)
            {
                if (_adaptorProviders.Count > 0)
                {
                   
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
