using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Discuss.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Layout.ViewAdaptors;

namespace Plato.Discuss.Channels.ViewAdaptors
{

    public class TopicListItemViewAdaptor : BaseAdaptorProvider
    {

        private readonly ICategoryStore<Channel> _channelStore;
        private readonly IFeatureFacade _featureFacade;

        public TopicListItemViewAdaptor(
            ICategoryStore<Channel> channelStore,
            IFeatureFacade featureFacade)
        {
            _channelStore = channelStore;
            _featureFacade = featureFacade;
        }

        public override Task<IViewAdaptorResult> ConfigureAsync()
        {

            var feature = _featureFacade.GetFeatureByIdAsync("Plato.Discuss.Channels")
                .GetAwaiter()
                .GetResult();

            if (feature == null)
            {
                // Return an anonymous type, we are adapting a view component
                return Task.FromResult(default(IViewAdaptorResult));
            }

            // Get all categories
            var channels = _channelStore.GetByFeatureIdAsync(feature.Id)
                .GetAwaiter()
                .GetResult();
            
            // Plato.Discuss does not have a dependency on Plato.Discuss.Channels
            // Instead we update the topic item view here via our view adaptor
            // This way the channel name is only ever populated if the channels feature is enabled
            return Adapt("TopicListItem", v =>
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

                    Channel channel = null;
                    if (channels != null)
                    {
                        channel = channels.FirstOrDefault(c => c.Id == model.Topic.CategoryId);
                    }

                    if (channel != null)
                    {
                        model.ChannelName = channel.Name;
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
