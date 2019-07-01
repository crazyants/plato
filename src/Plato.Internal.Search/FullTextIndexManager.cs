using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Plato.Internal.Search.Abstractions;

namespace Plato.Internal.Search
{
    
    public class FullTextIndexManager : IFullTextIndexManager
    {

        private IEnumerable<FullTextIndex> _indexes;

        private readonly IEnumerable<IFullTextIndexProvider> _providers;
        private readonly ILogger<FullTextIndexManager> _logger;

        public FullTextIndexManager(
            IEnumerable<IFullTextIndexProvider> providers,
            ILogger<FullTextIndexManager> logger)
        {
            _providers = providers;
            _logger = logger;
        }
        
        public IEnumerable<FullTextIndex> GetIndexes()
        {

            if (_indexes == null)
            {
                var indexes = new List<FullTextIndex>();
                foreach (var provider in _providers)
                {
                    try
                    {
                        indexes.AddRange(provider.GetIndexes());
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"An exception occurred within the full text index provider {provider.GetType().Name}. Please review your full text index provider and try again. {e.Message}");
                        throw;
                    }
                }

                _indexes = indexes;
            }

            return _indexes;
            
        }

        public IDictionary<string, IEnumerable<FullTextIndex>> GetIndexesByTable()
        {

            var output = new Dictionary<string, IEnumerable<FullTextIndex>>();
            foreach (var provider in _providers)
            {
                
                var indexes = provider.GetIndexes();
                foreach (var index in indexes)
                {
                    if (output.ContainsKey(index.TableName))
                    {
                        output[index.TableName] = output[index.TableName].Concat(new[] { index });
                    }
                    else
                    {
                        output.Add(index.TableName, new[] { index });
                    }
                }
            }

            return output;

        }

    }

}
