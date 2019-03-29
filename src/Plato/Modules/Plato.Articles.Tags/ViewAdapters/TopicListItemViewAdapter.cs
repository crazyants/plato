using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Plato.Articles.Models;
using Plato.Entities.Services;
using Plato.Entities.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Layout.ViewAdapters;
using Plato.Tags.Models;
using Plato.Tags.Stores;

namespace Plato.Articles.Tags.ViewAdapters
{

    public class ArticleListItemViewAdapter : BaseAdapterProvider
    {
        
        private readonly IEntityTagStore<EntityTag> _entityTagStore;
        private readonly IEntityService<Article> _entityService;
        private readonly IActionContextAccessor _actionContextAccessor;

        public ArticleListItemViewAdapter(
            IEntityService<Article> entityService, 
            IEntityTagStore<EntityTag> entityTagStore,
            IActionContextAccessor actionContextAccessor)
        {
            _entityService = entityService;
            _entityTagStore = entityTagStore;
            _actionContextAccessor = actionContextAccessor;
        }

        public override async Task<IViewAdapterResult> ConfigureAsync()
        {
            
            // Build a dictionary we can use below within our AdaptModel
            // method to add the correct tags for each displayed entity
            var entityLabelsDictionary = await BuildLookUpTable();
            
            // Plato.Discuss does not have a dependency on Plato.Discuss.Tags
            // Instead we update the model for the entity list item view component
            // here via our view adapter to include the tag data for the entity
            // This way the tag data is only ever populated if the tags feature is enabled
            return await Adapt("ArticleListItem", v =>
            {
                v.AdaptModel<EntityListItemViewModel<Article>>(model  =>
                {
                    if (model.Entity == null)
                    {
                        // Return an anonymous type as we are adapting a view component
                        return new
                        {
                            model
                        };
                    }

                    // No need to modify if we don't have a lookup table
                    if (entityLabelsDictionary == null)
                    {
                        // Return an anonymous type as we are adapting a view component
                        return new
                        {
                            model
                        };
                    }

                    // No need to modify the model if no labels have been found
                    if (!entityLabelsDictionary.ContainsKey(model.Entity.Id))
                    {
                        // Return an anonymous type as we are adapting a view component
                        return new
                        {
                            model
                        };
                    }

                    // Get labels for entity
                    var entityTags = entityLabelsDictionary[model.Entity.Id];

                    // Add labels to the model from our dictionary
                    var modelLabels = new List<EntityTag>();
                    foreach (var tag in entityTags)
                    {
                        modelLabels.Add(tag);
                    }

                    model.Tags = modelLabels;

                    // Return an anonymous type as we are adapting a view component
                    return new
                    {
                        model
                    };

                });
            });

        }
        
        async Task<IDictionary<int, IList<EntityTag>>> BuildLookUpTable()
        {
            
            // Get topic index view model from context
            var viewModel = _actionContextAccessor.ActionContext.HttpContext.Items[typeof(EntityIndexViewModel<Article>)] as EntityIndexViewModel<Article>;
            if (viewModel == null)
            {
                return null;
            }

            // Get all entities for our current view
            var entities = await _entityService.GetResultsAsync(
                viewModel?.Options, 
                viewModel?.Pager);

            // Get all entity tag relationships for displayed entities
            IPagedResults<EntityTag> entityTags = null;
            if (entities?.Data != null)
            {
                entityTags = await _entityTagStore.QueryAsync()
                    .Select<EntityTagQueryParams>(q =>
                    {
                        q.EntityId.IsIn(entities.Data.Select(e => e.Id).ToArray());
                    })
                    .ToList();
            }

            // Build a dictionary of entity and tag relationships
            var output = new ConcurrentDictionary<int, IList<EntityTag>>();
            if (entityTags?.Data != null)
            {
                foreach (var entityTag in entityTags.Data)
                {
                    var tag = entityTags.Data.FirstOrDefault(t => t.Id == entityTag.TagId);
                    if (tag != null)
                    {
                        output.AddOrUpdate(entityTag.EntityId, new List<EntityTag>()
                        {
                            tag
                        }, (k, v) =>
                        {
                            v.Add(tag);
                            return v;
                        });
                    }
                }
            }

            return output;

        }
        
    }
    
}
