using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Plato.Internal.Data.Abstractions;

namespace Plato.Internal.Data
{
    //public class FederatedQueryManager : IFederatedQueryManager
    //{

    //    private IEnumerable<string> _queries;

    //    private readonly IEnumerable<IFederatedQueryProvider> _providers;
    //    private readonly ILogger<FederatedQueryManager> _logger;

    //    public FederatedQueryManager(
    //        IEnumerable<IFederatedQueryProvider> providers,
    //        ILogger<FederatedQueryManager> logger)
    //    {
    //        _providers = providers;
    //        _logger = logger;
    //    }

    //    public IEnumerable<string> GetQueries(IFederatedQueryContext context)
    //    {
    //        if (_queries == null)
    //        {
    //            var queries = new List<string>();
    //            foreach (var provider in _providers)
    //            {
    //                try
    //                {

    //                    var providedQueries = provider.GetQueries(context);
    //                    foreach (var query in providedQueries)
    //                    {
    //                        queries.Add(ReplaceTablePrefix(query, context.Options.TablePrefix));
    //                    }
                   
    //                }
    //                catch (Exception e)
    //                {
    //                    _logger.LogError(e, $"An exception occurred within the search query provider {provider.GetType().Name}. Please review your query provider and try again. {e.Message}");
    //                    throw;
    //                }
    //            }

    //            _queries = queries;
    //        }

    //        return _queries;

    //    }

    //    string ReplaceTablePrefix(string input, string tablePrefix)
    //    {
    //        return input.Replace("{prefix}_", tablePrefix);
    //    }

    //}

}
