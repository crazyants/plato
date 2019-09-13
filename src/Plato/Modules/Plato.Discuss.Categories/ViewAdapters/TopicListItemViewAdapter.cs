using System;
using System.Linq;
using System.Threading.Tasks;
using Plato.Categories.Stores;
using Plato.Discuss.Categories.Models;
using Plato.Discuss.Models;
using Plato.Entities.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Layout.ViewAdapters;
using System.Collections.Generic;

namespace Plato.Discuss.Categories.ViewAdapters
{

    public class TopicListItemViewAdapter : BaseAdapterProvider
    {
             
        private readonly ICategoryStore<Category> _channelStore;
        private readonly IFeatureFacade _featureFacade;

        public TopicListItemViewAdapter(
            ICategoryStore<Category> channelStore,
            IFeatureFacade featureFacade)
        {
            _channelStore = channelStore;
            _featureFacade = featureFacade;
            ViewName = "TopicListItem";
        }

        IEnumerable<Category> _categories;

        public override async Task<IViewAdapterResult> ConfigureAsync(string viewName)
        {

            if (!viewName.Equals(ViewName, StringComparison.OrdinalIgnoreCase))
            {
                return default(IViewAdapterResult);
            }

            if (_categories == null)
            {
                // Get feature
                var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss.Categories");
                if (feature == null)
                {
                    // Feature not found
                    return default(IViewAdapterResult);
                }

                // Get all categories for feature
                _categories = await _channelStore.GetByFeatureIdAsync(feature.Id);
            }
            
            if (_categories == null)
            {
                // No categories available to adapt the view 
                return default(IViewAdapterResult);
            }
            
            // Plato.Discuss does not have a dependency on Plato.Discuss.Categories
            // Instead we update the model for the topic item view component
            // here via our view adapter to include the channel information
            // This way the channel data is only ever populated if the channels feature is enabled
            return await Adapt(ViewName, v =>
            {
                v.AdaptModel<EntityListItemViewModel<Topic>>(model =>
                {

                    if (model.Entity == null)
                    {
                        // Return an anonymous type, we are adapting a view component
                        return new
                        {
                            model
                        };
                    }

                    // Ensure we have a category
                    if (model.Entity.CategoryId <= 0)
                    {
                        // Return an anonymous type, we are adapting a view component
                        return new
                        {
                            model
                        };
                    }

                    // Get our category
                    var category = _categories.FirstOrDefault(c => c.Id == model.Entity.CategoryId);
                    if (category != null)
                    {
                        model.Category = category;
                    }
                    
                    // Return an anonymous type, we are adapting a view component
                    return new
                    {
                        model
                    };

                });
            });

        }

    }


}
