using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Plato.Discuss.Labels.Models;
using Plato.Discuss.Models;
using Plato.Entities.Services;
using Plato.Entities.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Layout.ViewAdapters;
using Plato.Labels.Models;
using Plato.Labels.Stores;

namespace Plato.Discuss.Labels.ViewAdapters
{

    public class TopicListItemViewAdapter : BaseAdapterProvider
    {
        
        private readonly ILabelStore<Label> _labelStore;
        private readonly IFeatureFacade _featureFacade;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEntityLabelStore<EntityLabel> _entityLabelStore;
        private readonly IEntityService<Topic> _entityService;
        private readonly IActionContextAccessor _actionContextAccessor;

        public TopicListItemViewAdapter(
            ILabelStore<Label> labelStore,
            IFeatureFacade featureFacade,
            IEntityService<Topic> entityService, 
            IEntityLabelStore<EntityLabel> entityLabelStore,
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor)
        {
            _labelStore = labelStore;
            _featureFacade = featureFacade;
            _entityService = entityService;
            _entityLabelStore = entityLabelStore;
            _httpContextAccessor = httpContextAccessor;
            _actionContextAccessor = actionContextAccessor;
        }

        public override async Task<IViewAdapterResult> ConfigureAsync()
        {
            
            // Get feature
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss.Labels");
            if (feature == null)
            {
                // Feature not found
                return default(IViewAdapterResult);
            }

            // Get all labels for feature
            var labels = await _labelStore.GetByFeatureIdAsync(feature.Id);
            if (labels == null)
            {
                // No labels available to adapt the view 
                return default(IViewAdapterResult);
            }

            // Build a dictionary we can use below within our AdaptModel
            // method to add the correct labels for each displayed entity
            var entityLabelsDictionary = await BuildLookUpTable(labels.ToList());
            
            // Plato.Discuss does not have a dependency on Plato.Discuss.Labels
            // Instead we update the model for the entity list item view component
            // here via our view adapter to include the label data for the entity
            // This way the label data is only ever populated if the labels feature is enabled
            return await Adapt("TopicListItem", v =>
            {
                v.AdaptModel<EntityListItemViewModel<Topic>>(model  =>
                {
                    if (model.Entity == null)
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
                    var entityLabels = entityLabelsDictionary[model.Entity.Id];

                    // Add labels to the model from our dictionary
                    var modelLabels = new List<Label>();
                    foreach (var label in entityLabels)
                    {
                        modelLabels.Add(label);
                    }

                    model.Labels = modelLabels;

                    // Return an anonymous type as we are adapting a view component
                    return new
                    {
                        model
                    };

                });
            });

        }
        
        async Task<IDictionary<int, IList<Label>>> BuildLookUpTable(IEnumerable<Label> labels)
        {
            
            // Get topic index view model from context
            var viewModel = _actionContextAccessor.ActionContext.HttpContext.Items[typeof(EntityIndexViewModel<Topic>)] as EntityIndexViewModel<Topic>;
        
            // Get all entities for our current view
            var entities = await _entityService.GetResultsAsync(
                viewModel?.Options, 
                viewModel?.Pager);

            // Get all entity label relationships for displayed entities
            IPagedResults<EntityLabel> entityLabels = null;
            if (entities?.Data != null)
            {
                entityLabels = await _entityLabelStore.QueryAsync()
                    .Select<EntityLabelQueryParams>(q =>
                    {
                        q.EntityId.IsIn(entities.Data.Select(e => e.Id).ToArray());
                    })
                    .ToList();
            }

            // Build a dictionary of entity and label relationships
            var output = new ConcurrentDictionary<int, IList<Label>>();
            if (entityLabels?.Data != null)
            {
                var labelList = labels.ToList();
                foreach (var entityLabel in entityLabels.Data)
                {
                    var label = labelList.FirstOrDefault(l => l.Id == entityLabel.LabelId);
                    if (label != null)
                    {
                        output.AddOrUpdate(entityLabel.EntityId, new List<Label>()
                        {
                            label
                        }, (k, v) =>
                        {
                            v.Add(label);
                            return v;
                        });
                    }
                }
            }

            return output;

        }
        
    }
    
}
