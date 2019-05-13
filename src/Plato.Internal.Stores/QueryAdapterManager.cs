using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions.QueryAdapters;

namespace Plato.Internal.Stores
{

    public class QueryAdapterManager<TModel> : IQueryAdapterManager<TModel> where TModel : class
    {
        
        private readonly IEnumerable<IQueryAdapterProvider<TModel>> _providers;
        private readonly ILogger<QueryAdapterManager<TModel>> _logger;

        public QueryAdapterManager(
            IEnumerable<IQueryAdapterProvider<TModel>> providers,
            ILogger<QueryAdapterManager<TModel>> logger)
        {
            _providers = providers;
            _logger = logger;
        }

        public void BuildSelect(IQuery<TModel> query, StringBuilder builder)
        {

            foreach (var provider in _providers)
            {
                try
                {
                    provider.BuildSelect(query, builder);
                }
                catch (Exception e)
                {
                    _logger.LogError(e,
                        $"An exception occurred within the BuildSelect method for query adapter provider {provider.GetType().Name}. Please review your query adapter and try again. {e.Message}");
                    throw;
                }
            }

            builder.Replace("{prefix}_", query.Options.TablePrefix);

        }

        public void BuildTables(IQuery<TModel> query, StringBuilder builder)
        {

            foreach (var provider in _providers)
            {
                try
                {
                    provider.BuildTables(query, builder);
                }
                catch (Exception e)
                {
                    _logger.LogError(e,
                        $"An exception occurred within the BuildTables method within the query adapter provider {provider.GetType().Name}. Please review your query adapter and try again. {e.Message}");
                    throw;
                }
            }

            builder.Replace("{prefix}_", query.Options.TablePrefix);

        }

        public void BuildWhere(IQuery<TModel> query, StringBuilder builder)
        {

            foreach (var provider in _providers)
            {
                try
                {
                    provider.BuildWhere(query, builder);
                }
                catch (Exception e)
                {
                    _logger.LogError(e,
                        $"An exception occurred within the BuildWhere method of the query adapter provider {provider.GetType().Name}. Please review your query adapter and try again. {e.Message}");
                    throw;
                }
            }

            builder.Replace("{prefix}_", query.Options.TablePrefix);
            
        }
        
    }

}