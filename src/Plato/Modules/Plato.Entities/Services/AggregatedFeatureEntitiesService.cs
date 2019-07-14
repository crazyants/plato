using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Metrics;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Entities.Services
{

    public interface IAggregatedFeatureEntitiesService
    {

        Task<IPagedResults<AggregatedCount<string>>> GetResultsAsync(EntityIndexOptions options);

        IAggregatedFeatureEntitiesService ConfigureDb(Action<IQueryOptions> configure);

        IAggregatedFeatureEntitiesService ConfigureQuery(Action<AggregatedEntityQueryParams> configure);
    }

    public class AggregatedFeatureEntitiesService : IAggregatedFeatureEntitiesService
    {

        private Action<QueryOptions> _configureDb = null;
        private Action<AggregatedEntityQueryParams> _configureParams = null;

        private readonly IAggregatedFeatureEntitiesStore _aggregatedFeatureEntitiesStore;

        public AggregatedFeatureEntitiesService(
            IAggregatedFeatureEntitiesStore aggregatedFeatureEntitiesStore)
        {
            _aggregatedFeatureEntitiesStore = aggregatedFeatureEntitiesStore;

            // Default options delegate
            _configureDb = options => options.SearchType = SearchTypes.Tsql;

        }

        public IAggregatedFeatureEntitiesService ConfigureDb(Action<IQueryOptions> configure)
        {
            _configureDb = configure;
            return this;
        }
        
        public IAggregatedFeatureEntitiesService ConfigureQuery(Action<AggregatedEntityQueryParams> configure)
        {
            _configureParams = configure;
            return this;
        }

        public async Task<IPagedResults<AggregatedCount<string>>> GetResultsAsync(EntityIndexOptions options)
        {
      
            // Return tailored results
            return await _aggregatedFeatureEntitiesStore.QueryAsync()
                .Configure(_configureDb)
                .Select<AggregatedEntityQueryParams>(q =>
                {
                    
                    // ----------------
                    // Basic parameters
                    // ----------------

                    if (options.FeatureId != null && options.FeatureId.Value > 0)
                    {
                        q.FeatureId.Equals(options.FeatureId.Value);
                    }

                    if (!string.IsNullOrEmpty(options.Search))
                    {
                        q.Keywords.Like(options.Search);
                    }

                    // Multiple categories
                    if (options.CategoryIds != null)
                    {
                        q.CategoryId.IsIn(options.CategoryIds);
                    }
                    else
                    {
                        // A single category
                        if (options.CategoryId >= 0)
                        {
                            q.CategoryId.Equals(options.CategoryId);
                        }
                    }

                    if (options.LabelId > 0)
                    {
                        q.LabelId.Equals(options.LabelId);
                    }

                    if (options.TagId > 0)
                    {
                        q.TagId.Equals(options.TagId);
                    }

                    if (options.CreatedByUserId > 0)
                    {
                        q.CreatedUserId.Equals(options.CreatedByUserId);
                    }

                    // ----------------
                    // Additional parameter configuration
                    // ----------------

                    _configureParams?.Invoke(q);

                })
                .OrderBy(options.SortColumns)
                .ToList();


        }

    }

}
