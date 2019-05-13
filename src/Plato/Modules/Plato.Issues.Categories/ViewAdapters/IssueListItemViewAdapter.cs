using System.Linq;
using System.Threading.Tasks;
using Plato.Categories.Stores;
using Plato.Issues.Categories.Models;
using Plato.Issues.Models;
using Plato.Entities.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Layout.ViewAdapters;

namespace Plato.Issues.Categories.ViewAdapters
{

    public class IssueListItemViewAdapter : BaseAdapterProvider
    {

        private readonly ICategoryStore<Category> _channelStore;
        private readonly IFeatureFacade _featureFacade;

        public IssueListItemViewAdapter(
            ICategoryStore<Category> channelStore,
            IFeatureFacade featureFacade)
        {
            _channelStore = channelStore;
            _featureFacade = featureFacade;
        }

        public override async Task<IViewAdapterResult> ConfigureAsync()
        {

            // Get feature
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Issues.Categories");
            if (feature == null)
            {
                // Feature not found
                return default(IViewAdapterResult);
            }

            // Get all categories for feature
            var channels = await _channelStore.GetByFeatureIdAsync(feature.Id);
            if (channels == null)
            {
                // No categories available to adapt the view 
                return default(IViewAdapterResult);
            }
            
            // Plato.Issues does not have a dependency on Plato.Issues.Categories
            // Instead we update the model for the topic item view component
            // here via our view adapter to include the channel information
            // This way the channel data is only ever populated if the channels feature is enabled
            return await Adapt("IssueListItem", v =>
            {
                v.AdaptModel<EntityListItemViewModel<Issue>>(model =>
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

                    // Get our channel
                    var channel = channels.FirstOrDefault(c => c.Id == model.Entity.CategoryId);
                    if (channel != null)
                    {
                        model.Category = channel;
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
