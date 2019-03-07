using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Internal.Layout.ViewAdapters;
using Plato.Moderation.Models;

namespace Plato.Discuss.Moderation.ViewAdapters
{

    public class ModerationViewAdapterProvider : BaseAdapterProvider
    {

        private readonly ICategoryStore<ChannelHome> _channelStore;

        private IHtmlLocalizer T { get; }
        
        public ModerationViewAdapterProvider(
            ICategoryStore<ChannelHome> channelStore,
            IHtmlLocalizer htmlLocalizer)
        {
            _channelStore = channelStore;
            T = htmlLocalizer;
        }

        public override Task<IViewAdapterResult> ConfigureAsync()
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
                        model.CategoryName = sb.ToString();
                    }
                    else
                    {
                        model.CategoryName = T["All Channels"].Value.ToString();
                    }
                    
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
