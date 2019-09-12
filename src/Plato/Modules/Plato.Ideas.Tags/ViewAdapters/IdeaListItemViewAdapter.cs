using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Plato.Ideas.Models;
using Plato.Entities.Services;
using Plato.Entities.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Layout.ViewAdapters;
using Plato.Tags.Models;
using Plato.Tags.Stores;
using System;

namespace Plato.Ideas.Tags.ViewAdapters
{

    public class IdeaListItemViewAdapter : BaseAdapterProvider
    {

  
        private readonly IEntityTagStore<EntityTag> _entityTagStore;
        private readonly IEntityService<Idea> _entityService;
        private readonly IActionContextAccessor _actionContextAccessor;

        public IdeaListItemViewAdapter(
            IEntityService<Idea> entityService, 
            IEntityTagStore<EntityTag> entityTagStore,
            IActionContextAccessor actionContextAccessor)
        {
            _entityService = entityService;
            _entityTagStore = entityTagStore;
            _actionContextAccessor = actionContextAccessor;
            ViewName = "IdeaListItem";
        }

        public override async Task<IViewAdapterResult> ConfigureAsync(string viewName)
        {

            if (!viewName.Equals(ViewName, StringComparison.OrdinalIgnoreCase))
            {
                return default(IViewAdapterResult);
            }

            // Build a dictionary we can use below within our AdaptModel
            // method to add the correct tags for each displayed entity
            var entityTagsDictionary = await BuildLookUpTable();
            
            // Plato.Discuss does not have a dependency on Plato.Discuss.Tags
            // Instead we update the model for the entity list item view component
            // here via our view adapter to include the tag data for the entity
            // This way the tag data is only ever populated if the tags feature is enabled
            return await Adapt(ViewName, v =>
            {
                v.AdaptModel<EntityListItemViewModel<Idea>>(model  =>
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
                    if (entityTagsDictionary == null)
                    {
                        // Return an anonymous type as we are adapting a view component
                        return new
                        {
                            model
                        };
                    }

                    // No need to modify the model if no labels have been found
                    if (!entityTagsDictionary.ContainsKey(model.Entity.Id))
                    {
                        // Return an anonymous type as we are adapting a view component
                        return new
                        {
                            model
                        };
                    }

                    // Add tags to the model from our dictionary
                    var entityTags = new List<EntityTag>();
                    if (entityTagsDictionary.ContainsKey(model.Entity.Id))
                    {
                        foreach (var tag in entityTagsDictionary[model.Entity.Id])
                        {
                            entityTags.Add(tag);
                        }
                    }
                    
                    model.Tags = entityTags;

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
            var viewModel = _actionContextAccessor.ActionContext.HttpContext.Items[typeof(EntityIndexViewModel<Idea>)] as EntityIndexViewModel<Idea>;
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
                        q.EntityReplyId.Equals(0);
                    })
                    .ToList();
            }

            // Build a dictionary of entity and tag relationships
            var output = new ConcurrentDictionary<int, IList<EntityTag>>();
            if (entityTags?.Data != null)
            {
                foreach (var entityTag in entityTags.Data)
                {
                    var tag = entityTags.Data.FirstOrDefault(t => t.TagId == entityTag.TagId);
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
