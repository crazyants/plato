using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Internal.Stores
{

    public class QueryAdapterManager<TModel> : IQueryAdapterManager<TModel> where TModel : class
    {

        private IEnumerable<string> _queries;

        private readonly IEnumerable<IQueryAdapterProvider<TModel>> _providers;
        private readonly ILogger<QueryAdapterManager<TModel>> _logger;

        public QueryAdapterManager(
            IEnumerable<IQueryAdapterProvider<TModel>> providers,
            ILogger<QueryAdapterManager<TModel>> logger)
        {
            _providers = providers;
            _logger = logger;
        }

        public IEnumerable<string> GetAdaptations(TModel queryParams)
        {
            if (_queries == null)
            {
                var queries = new List<string>();
                foreach (var provider in _providers)
                {
                    try
                    {
                        queries.Add(provider.AdaptQuery(queryParams));
                       
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"An exception occurred within the search query provider {provider.GetType().Name}. Please review your query provider and try again. {e.Message}");
                        throw;
                    }
                }

                _queries = queries;
            }

            return _queries;

        }


    }

}