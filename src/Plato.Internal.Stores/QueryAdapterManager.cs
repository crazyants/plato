using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions.QueryAdapters;

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

        public IEnumerable<string> GetAdaptations(IQuery<TModel> query)
        {
            if (_queries == null)
            {
                var queries = new List<string>();
                foreach (var provider in _providers)
                {
                    try
                    {
                        var adaptation = provider.AdaptQuery(query);
                        if (!string.IsNullOrEmpty(adaptation))
                        {
                            adaptation = ReplaceTablePrefix(adaptation, query.Options.TablePrefix);
                            if (!queries.Contains(adaptation))
                            {
                                queries.Add(adaptation);
                            }
                            
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"An exception occurred within the query adapter provider {provider.GetType().Name}. Please review your query adapter and try again. {e.Message}");
                        throw;
                    }
                }

                _queries = queries;
            }

            return _queries;

        }

        string ReplaceTablePrefix(string input, string tablePrefix)
        {
            return input.Replace("{prefix}_", tablePrefix);
        }


    }

}