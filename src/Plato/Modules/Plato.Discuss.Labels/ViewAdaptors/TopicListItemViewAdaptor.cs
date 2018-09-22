using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Plato.Discuss.Labels.Models;
using Plato.Discuss.Services;
using Plato.Discuss.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Layout.ViewAdaptors;
using Plato.Internal.Navigation;
using Plato.Labels.Models;
using Plato.Labels.Stores;

namespace Plato.Discuss.Labels.ViewAdaptors
{

    public class TopicListItemViewAdaptor : BaseAdaptorProvider
    {

   
        private readonly ILabelStore<Label> _labelStore;
        private readonly IFeatureFacade _featureFacade;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IEntityLabelStore<EntityLabel> _entityLabelStore;
        private readonly ITopicService _topicService;

        public TopicListItemViewAdaptor(
            ILabelStore<Label> labelStore,
            IFeatureFacade featureFacade,
            IActionContextAccessor actionContextAccessor,
            ITopicService topicService, 
            IEntityLabelStore<EntityLabel> entityLabelStore)
        {
            _labelStore = labelStore;
            _featureFacade = featureFacade;
            _actionContextAccessor = actionContextAccessor;
            _topicService = topicService;
            _entityLabelStore = entityLabelStore;
        }

        public override async Task<IViewAdaptorResult> ConfigureAsync()
        {

            // Get feature
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss.Labels");
            if (feature == null)
            {
                // Feature not found
                return default(IViewAdaptorResult);
            }

            // Get all labels for feature
            var labels = await _labelStore.GetByFeatureIdAsync(feature.Id);
            if (labels == null)
            {
                // No categories available to adapt the view 
                return default(IViewAdaptorResult);
            }

            // ---------------------

            // Get acttion parameters for current view
            var descriptor = _actionContextAccessor.ActionContext.ActionDescriptor;
            var options = descriptor.GetProperty<TopicIndexOptions>();
            var pager = descriptor.GetProperty<PagerOptions>();
            
            // Get all entities for our current view
            var entities = await _topicService.Get(options, pager);

            IPagedResults<EntityLabel> entityLabels = null;
            if (entities?.Data != null)
            {
                // Get all entity label relationships for displayed entities
                entityLabels = await _entityLabelStore.QueryAsync()
                    .Select<EntityLabelQueryParams>(q => { q.EntityId.IsIn(entities.Data.Select(e => e.Id).ToArray()); })
                    .ToList();
            }

            // Build a ditionary of entity and label relationships
            var lookUpTable = new ConcurrentDictionary<int, IList<Label>>();
            if (entityLabels?.Data != null)
            {
                var labelList = labels.ToList();
                foreach (var entityLabel in entityLabels.Data)
                {
                    var label = labelList.FirstOrDefault(l => l.Id == entityLabel.LabelId);
                    lookUpTable.AddOrUpdate(entityLabel.Id, new List<Label>()
                    {
                        label
                    }, (k, v) =>
                    {
                        v.Add(label);
                        return v;
                    });
                }
            }

            // ---------------------

            // Plato.Discuss does not have a dependency on Plato.Discuss.Labels
            // Instead we update the model for the topic item view component
            // here via our view adaptor to include the label information
            // This way the label data is only ever populated if the labels feature is enabled
            return await Adapt("TopicListItem", v =>
            {
                v.AdaptModel<TopicListItemViewModel>(model =>
                {

                    if (model.Topic == null)
                    {
                        // Return an anonymous type, we are adapting a view component
                        return new
                        {
                            model = model
                        };
                    }
                    
                    // No need to modify the model if no labels have been found
                    var topicLabels = lookUpTable[model.Topic.Id];
                    if (topicLabels == null)
                    {
                        // Return an anonymous type, we are adapting a view component
                        return new
                        {
                            model = model
                        };
                    }

                    // Add labels to the model from our dictionary
                    var modelLabels = new List<Label>();
                    foreach (var label in topicLabels)
                    {
                        modelLabels.Add(label);
                    }

                    model.Labels = modelLabels;

                    // Return an anonymous type, we are adapting a view component
                    return new
                    {
                        model = model
                    };

                });
            });

        }

    }


}
