using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Entities.Services
{
    
    public class EntityService<TModel> : IEntityService<TModel> where TModel : class, IEntity
    {

        private Action<QueryOptions> _configureDb = null;
        private Action<EntityQueryParams> _configureParams = null;
        
        private readonly IContextFacade _contextFacade;
        private readonly IEntityStore<TModel> _entityStore;
  
        public EntityService(
            IContextFacade contextFacade,
            IEntityStore<TModel> entityStore)
        {
            _contextFacade = contextFacade;
            _entityStore = entityStore;
       
            // Default options delegate
            _configureDb = options => options.SearchType = SearchTypes.Tsql;

        }
        
        public IEntityService<TModel> ConfigureDb(Action<IQueryOptions> configure)
        {
            _configureDb = configure;
            return this;
        }
        
        public IEntityService<TModel> ConfigureQuery(Action<EntityQueryParams> configure)
        {
            _configureParams = configure;
            return this;
        }
        

        public async Task<IPagedResults<TModel>> GetResultsAsync(EntityIndexOptions options, PagerOptions pager)
        {

            if (options == null)
            {
                options = new EntityIndexOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }

         
            // Ensure we have a sort column is non is specified
            if (options.Sort == SortBy.Auto)
            {
                options.Sort = SortBy.LastReply;
            }
            
            // Our list of columns to sort by
            var sortColumns = new Dictionary<string, OrderBy>();

            // Allow for additional sort columns
            if (options.SortColumns != null)
            {
                foreach (var column in options.SortColumns)
                {
                    if (!sortColumns.ContainsKey(column.Key))
                    {
                        sortColumns.Add(column.Key, column.Value);
                    }
                }
            }

            // Sort by our primary column
            sortColumns.Add(options.Sort.ToString(), options.Order);
  
            // Get authenticated user 
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            
            // Return tailored results
            return await _entityStore.QueryAsync()
                .Take(pager.Page, pager.Size)
                .Configure(_configureDb)
                .Select<EntityQueryParams>(q =>
                {
                    
                    // ----------------
                    // Set current authenticated user id
                    // This is required for various security checks
                    // i.e. Role based security & displaying private entities
                    // ----------------

                    q.UserId.Equals(user?.Id ?? 0);

                    // ----------------
                    // Basic parameters
                    // ----------------

                    if (options.FeatureId != null && options.FeatureId.Value > 0)
                        q.FeatureId.Equals(options.FeatureId.Value);

                    if (!string.IsNullOrEmpty(options.Search))
                        q.Keywords.Like(options.Search);

                    if (options.CategoryId >= 0)
                        q.CategoryId.Equals(options.CategoryId);

                    if (options.CategoryIds != null)
                        q.CategoryId.IsIn(options.CategoryIds);

                    if (options.LabelId > 0)
                        q.LabelId.Equals(options.LabelId);

                    if (options.TagId > 0)
                        q.TagId.Equals(options.TagId);

                    if (options.CreatedByUserId > 0)
                        q.CreatedUserId.Equals(options.CreatedByUserId);
                    
                    // ----------------
                    // Filters
                    // ----------------

                    switch (options.Filter)
                    {
                        case FilterBy.Started:
                            if (user != null)
                            {
                                q.CreatedUserId.Equals(user.Id);
                            }

                            break;
                        case FilterBy.Participated:
                            if (user != null)
                            {
                                q.ParticipatedUserId.Equals(user.Id);
                            }

                            break;
                        case FilterBy.Following:
                            if (user != null)
                            {
                                q.FollowUserId.Equals(user.Id, b =>
                                {
                                    // Restrict follows by topic
                                    b.Append(" AND f.[Name] = 'Topic'");
                                });
                            }

                            break;
                        case FilterBy.Starred:
                            if (user != null)
                            {
                                q.StarUserId.Equals(user.Id, b =>
                                {
                                    // Restrict stars by topic
                                    b.Append(" AND s.[Name] = 'Topic'");
                                });
                            }

                            break;

                        case FilterBy.NoReplies:

                            q.TotalReplies.Equals(0);
                            break;
                    }
                    
                    // ----------------
                    // Additional parameter configuration
                    // ----------------

                    _configureParams?.Invoke(q);
                    
                })
                .OrderBy(sortColumns)
                .ToList();


        }

    }
}
