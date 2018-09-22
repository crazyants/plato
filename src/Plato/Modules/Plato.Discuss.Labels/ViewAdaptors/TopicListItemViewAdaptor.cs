using System.Linq;
using System.Threading.Tasks;
using Plato.Discuss.Labels.Models;
using Plato.Discuss.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Layout.ViewAdaptors;
using Plato.Labels.Stores;

namespace Plato.Discuss.Labels.ViewAdaptors
{

    public class TopicListItemViewAdaptor : BaseAdaptorProvider
    {

        private readonly ILabelStore<Label> _labelStore;
        private readonly IFeatureFacade _featureFacade;

        public TopicListItemViewAdaptor(
            ILabelStore<Label> labelStore,
            IFeatureFacade featureFacade)
        {
            _labelStore = labelStore;
            _featureFacade = featureFacade;
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
            var channels = await _labelStore.GetByFeatureIdAsync(feature.Id);
            if (channels == null)
            {
                // No categories available to adapt the view 
                return default(IViewAdaptorResult);
            }
            
            // Plato.Discuss does not have a dependency on Plato.Discuss.Channels
            // Instead we update the model for the topic item view component
            // here via our view adaptor to include the channel information
            // This way the channel data is only ever populated if the channels feature is enabled
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

                    // Ensure we have a category
                    if (model.Topic.CategoryId <= 0)
                    {
                        // Return an anonymous type, we are adapting a view component
                        return new
                        {
                            model = model
                        };
                    }

                    // Get our channel
                    var channel = channels.FirstOrDefault(c => c.Id == model.Topic.CategoryId);
                    if (channel != null)
                    {
                        model.Channel = channel;
                    }
                    
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
