using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Entities.Services
{
    
    public class EntityReplyService<TModel> : IEntityReplyService<TModel> where TModel : class, IEntityReply
    {

        private Action<QueryOptions> _configureDb = null;
        private Action<EntityReplyQueryParams> _configureParams = null;

        private readonly IEntityReplyStore<TModel> _entityReplyStore;
  
        public EntityReplyService(
            IEntityReplyStore<TModel> entityReplyStore)
        {
            _entityReplyStore = entityReplyStore;

            // Default options delegate
            _configureDb = options => options.SearchType = SearchTypes.Tsql;
        }

        public IEntityReplyService<TModel> ConfigureDb(Action<QueryOptions> configure)
        {
            _configureDb = configure;
            return this;
        }
        
        public IEntityReplyService<TModel> ConfigureQuery(Action<EntityReplyQueryParams> configure)
        {
            _configureParams = configure;
            return this;
        }
        
        public async Task<IPagedResults<TModel>> GetResultsAsync(EntityOptions options, PagerOptions pager)
        {
            
            // Build sort columns from model
            var sortColumns = new Dictionary<string, OrderBy>();
            if (options.SortColumns != null)
            {
                foreach (var column in options.SortColumns)
                {
                    sortColumns.Add(column.Key, column.Value);
                }
            }
            else
            {
                sortColumns.Add(options.Sort, options.Order);
            }
            
            return await _entityReplyStore.QueryAsync()
                .Take(pager.Page, pager.Size)
                .Configure(_configureDb)
                .Select<EntityReplyQueryParams>(q =>
                {
                    q.EntityId.Equals(options.Id);
                    
                    // Additional parameter configuration
                    _configureParams?.Invoke(q);
                    
                })
                .OrderBy(sortColumns)
                .ToList();

        }

    }

}
