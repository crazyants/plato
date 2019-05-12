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

        private IEnumerable<string> _selects;
        private IEnumerable<string> _tables;
        private IEnumerable<string> _wheres;
      
        private readonly IEnumerable<IQueryAdapterProvider<TModel>> _providers;
        private readonly ILogger<QueryAdapterManager<TModel>> _logger;

        public QueryAdapterManager(
            IEnumerable<IQueryAdapterProvider<TModel>> providers,
            ILogger<QueryAdapterManager<TModel>> logger)
        {
            _providers = providers;
            _logger = logger;
        }

        public IEnumerable<string> BuildSelect(IQuery<TModel> query, StringBuilder builder)
        {
            if (_selects == null)
            {
                var selects = new List<string>();
                foreach (var provider in _providers)
                {
                    try
                    {
                        provider.BuildSelect(query, builder);
                        builder.Replace("{prefix}_", query.Options.TablePrefix);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"An exception occurred within the BuildSelect method for query adapter provider {provider.GetType().Name}. Please review your query adapter and try again. {e.Message}");
                        throw;
                    }
                }

                _selects = selects;
            }

            return _tables;
        }

        public IEnumerable<string> BuildTables(IQuery<TModel> query, StringBuilder builder)
        {
            if (_tables == null)
            {
                var tables = new List<string>();
                foreach (var provider in _providers)
                {
                    try
                    {
                        provider.BuildTables(query, builder);
                        builder.Replace("{prefix}_", query.Options.TablePrefix);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"An exception occurred within the BuildTables method within the query adapter provider {provider.GetType().Name}. Please review your query adapter and try again. {e.Message}");
                        throw;
                    }
                }

                _tables = tables;
            }

            return _tables;

        }
        
        public IEnumerable<string> BuildWhere(IQuery<TModel> query, StringBuilder builder)
        {
            if (_wheres == null)
            {
                var queries = new List<string>();
                foreach (var provider in _providers)
                {
                    try
                    {
                        provider.BuildWhere(query, builder);
                        builder.Replace("{prefix}_", query.Options.TablePrefix);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"An exception occurred within the BuildWhere method of the query adapter provider {provider.GetType().Name}. Please review your query adapter and try again. {e.Message}");
                        throw;
                    }
                }

                _wheres = queries;
            }

            return _wheres;

        }

        StringBuilder ReplaceTablePrefix(StringBuilder input, string tablePrefix)
        {
            return input.Replace("{prefix}_", tablePrefix);
        }

    }

}