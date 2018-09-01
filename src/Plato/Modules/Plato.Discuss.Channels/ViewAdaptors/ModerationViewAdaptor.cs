using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            // Plato.Discuss.Moderation does not have a dependency on Plato.Discuss.Channels
            // Instead we update the moderator view here via our view adaptor
            // This way the channel name is only ever populated if the channels feature is enabled
            return Adapt("ModeratorListItem",  v =>
            {
                v.AdaptModel<Moderator>(model =>
                {


                    IEnumerable<Channel> parents = null;
                    if (model.CategoryId > 0)
                    {
                        parents = _channelStore.GetParentsByIdAsync(model.CategoryId)
                            .GetAwaiter()
                            .GetResult();
                    }

                    var sb = new StringBuilder();
                    if (parents != null)
                    {
                        var i = 0;
                        var parentList = parents.ToList();
                        foreach (var parent in parentList)
                        {
                            sb.Append(parent.Name);
                            if (i < parentList.Count - 1)
                            {
                                sb.Append(" / ");
                            }
                            i += 1;
                        }
                    }
                    model.CategoryName = sb.ToString();
                    // Return an anonymous type, we are adapting a view component
                    return new
                    {
                        moderator = model
                    };
                });
            });
            
        }
        
    }


}
