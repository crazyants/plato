using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;

namespace Plato.Entities.Services
{

    public class FeatureEntityCountService : IFeatureEntityCountService
    {

        private Action<QueryOptions> _configureDb = null;
        private Action<FeatureEntityCountQueryParams> _configureParams = null;
        
        private readonly IFeatureEntityCountStore _featureEntityCountStore;
        private readonly IContextFacade _contextFacade;

        public FeatureEntityCountService(
            IFeatureEntityCountStore featureEntityCountStore,
            IContextFacade contextFacade)
        {
            _featureEntityCountStore = featureEntityCountStore;
            _contextFacade = contextFacade;

            // Default options delegate
            _configureDb = options => options.SearchType = SearchTypes.Tsql;

        }

        public IFeatureEntityCountService ConfigureDb(Action<IQueryOptions> configure)
        {
            _configureDb = configure;
            return this;
        }
        
        public IFeatureEntityCountService ConfigureQuery(Action<FeatureEntityCountQueryParams> configure)
        {
            _configureParams = configure;
            return this;
        }

        public async Task<IEnumerable<FeatureEntityCount>> GetResultsAsync(EntityIndexOptions options = null)
        {

            if (options == null)
            {
                options = new EntityIndexOptions();
            }

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Return tailored results
            var results = await _featureEntityCountStore.QueryAsync()
                .Configure(_configureDb)
                .Select<FeatureEntityCountQueryParams>(q =>
                {

                    // ----------------
                    // Required for role based security checks
                    // ----------------

                    if (user != null)
                    {
                        q.UserId.Equals(user.Id);
                    }
                    

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

            return results?.Data;

        }

    }

}
