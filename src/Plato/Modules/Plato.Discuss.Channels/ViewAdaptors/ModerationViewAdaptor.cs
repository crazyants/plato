using System.Threading.Tasks;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Internal.Layout.ViewAdaptors;
using Plato.Moderation.Models;

namespace Plato.Discuss.Channels.ViewAdaptors
{

    public class ModerationViewAdaptorProvider : BaseAdaptorProvider
    {

        private readonly ICategoryStore<Channel> _channelStore;
      

        public ModerationViewAdaptorProvider(ICategoryStore<Channel> channelStore)
        {
            _channelStore = channelStore;
        }

        public override Task<IViewAdaptorResult> ConfigureAsync()
        {

            // lato.Discuss.Moderation does not have a dependency on Plato.Discuss.Channels
            // Instead we update the moderator view here via our view adaptor
            // This way the channel name is only ever populated if the channels feature is enabled
            return Adapt("ModeratorListItem",  v =>
            {
                v.AdaptModel<Moderator>(model =>
                {
                    Channel channel = null;
                    if (model.CategoryId > 0)
                    {
                        channel = _channelStore.GetByIdAsync(model.CategoryId).GetAwaiter().GetResult();
                    }
                    if (channel != null)
                    {
                        model.CategoryName = channel.Name;
                    }
                    // Ensure we return an anonymous type
                    // as we are adapting a view component
                    return new
                    {
                        moderator = model
                    };
                });
            });
            
        }
        
    }


}
