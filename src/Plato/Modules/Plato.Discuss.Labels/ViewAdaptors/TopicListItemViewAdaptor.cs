using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Plato.Discuss.Labels.Models;
using Plato.Discuss.Services;
using Plato.Discuss.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Layout.ViewAdaptors;
using Plato.Labels.Models;
using Plato.Labels.Stores;

namespace Plato.Discuss.Labels.ViewAdaptors
{

    public class TopicListItemViewAdaptor : BaseAdaptorProvider
    {

        //private IDictionary<int, IList<Label>> _lookUpTable;

        private readonly ILabelStore<Label> _labelStore;
        private readonly IFeatureFacade _featureFacade;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEntityLabelStore<EntityLabel> _entityLabelStore;
        private readonly ITopicService _topicService;
        private readonly IActionContextAccessor _actionContextAccessor;

        public TopicListItemViewAdaptor(
            ILabelStore<Label> labelStore,
            IFeatureFacade featureFacade,
            ITopicService topicService, 
            IEntityLabelStore<EntityLabel> entityLabelStore,
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor)
        {
            _labelStore = labelStore;
            _featureFacade = featureFacade;
            _topicService = topicService;
            _entityLabelStore = entityLabelStore;
            _httpContextAccessor = httpContextAccessor;
            _actionContextAccessor = actionContextAccessor;
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
                // No labels available to adapt the view 
                return default(IViewAdaptorResult);
            }

            // Build a dictionary we can use below within our AdaptModel
            // method to add the correct labels for each entitty
            var topicLabelsDictionary = await BuildLoookUpTable(labels.ToList());
            
            // Plato.Discuss does not have a dependency on Plato.Discuss.Labels
            // Instead we update the model for the topic item view component
            // here via our view adaptor to include the label information
            // This way the label data is only ever populated if the labels feature is enabled
            return await Adapt("TopicListItem", v =>
            {
                v.AdaptModel<TopicListItemViewModel>(model  =>
                {

                    //if (_lookUpTable == null)
                    //{
                    //    _lookUpTable = BuildLoookUpTable(labels)
                    //        .GetAwaiter()
                    //        .GetResult();
                    //}

                    if (model.Topic == null)
                    {
                        // Return an anonymous type, we are adapting a view component
                        return new
                        {
                            model = model
                        };
                    }
                    
                    // No need to modify the model if no labels have been found
                    if (!topicLabelsDictionary.ContainsKey(model.Topic.Id))
                    {
                        // Return an anonymous type, we are adapting a view component
                        return new
                        {
                            model = model
                        };
                    }

                    // Get labels for entity
                    var topicLabels = topicLabelsDictionary[model.Topic.Id];

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
        
        async Task<IDictionary<int, IList<Label>>> BuildLoookUpTable(IEnumerable<Label> labels)
        {
            
            // Get topic index view model from context
            var viewModel = _actionContextAccessor.ActionContext.HttpContext.Items[typeof(TopicIndexViewModel)] as TopicIndexViewModel;
        
            // Get all entities for our current view
            var entities = await _topicService.Get(viewModel?.Options, viewModel?.Pager);

            // Get all entity label relationships for displayed entities
            IPagedResults<EntityLabel> entityLabels = null;
            if (entities?.Data != null)
            {
                entityLabels = await _entityLabelStore.QueryAsync()
                    .Select<EntityLabelQueryParams>(q => { q.EntityId.IsIn(entities.Data.Select(e => e.Id).ToArray()); })
                    .ToList();
            }

            // Build a ditionary of entity and label relationships
            var output = new ConcurrentDictionary<int, IList<Label>>();
            if (entityLabels?.Data != null)
            {
                var labelList = labels.ToList();
                foreach (var entityLabel in entityLabels.Data)
                {
                    var label = labelList.FirstOrDefault(l => l.Id == entityLabel.LabelId);
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

            return output;

        }
        

    }


}
