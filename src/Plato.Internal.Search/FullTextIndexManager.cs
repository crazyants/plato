using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                var permissions = new List<FullTextIndex>();
                foreach (var provider in _providers)
                {
                    try
                    {
                        permissions.AddRange(provider.GetIndexes());
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"An exception occurred within the full text index provider {provider.GetType().Name}. Please review your full text index provider and try again. {e.Message}");
                        throw;
                    }
                }

                _indexes = permissions;
            }

            return _indexes;


        }

        public IDictionary<string, IEnumerable<FullTextIndex>> GetIndexesByTable()
        {

            var output = new Dictionary<string, IEnumerable<FullTextIndex>>();

            foreach (var provider in _providers)
            {
                
                var permissions = provider.GetIndexes();
                foreach (var permission in permissions)
                {
                    var category = permission.TableName;
                    var title = String.IsNullOrWhiteSpace(category) ?
                        "" :
                        category;

                    if (output.ContainsKey(title))
                    {
                        output[title] = output[title].Concat(new[] { permission });
                    }
                    else
                    {
                        output.Add(title, new[] { permission });
                    }
                }
            }

            return output;

        }

    }

}
