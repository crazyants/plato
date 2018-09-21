using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Discuss.ViewModels;
using Plato.Internal.Layout.ViewAdaptors;
using Plato.Moderation.Models;

namespace Plato.Discuss.Channels.ViewAdaptors
{
    
    public class TopicListItemViewAdaptor : BaseAdaptorProvider
    {

        private readonly ICategoryStore<Channel> _channelStore;

        private IHtmlLocalizer T { get; }

        public TopicListItemViewAdaptor(
            ICategoryStore<Channel> channelStore,
            IHtmlLocalizer htmlLocalizer)
        {
            _channelStore = channelStore;
            T = htmlLocalizer;
        }

        public override Task<IViewAdaptorResult> ConfigureAsync()
        {

            // Plato.Discuss does not have a dependency on Plato.Discuss.Channels
            // Instead we update the topic item view here via our view adaptor
            // This way the channel name is only ever populated if the channels feature is enabled
            return Adapt("TopicListItem", v =>
            {
                v.AdaptModel<TopicListItemViewModel>(model =>
                {

                    IEnumerable<Channel> parents = null;
                    if (model.Topic.CategoryId > 0)
                    {
                        parents = _channelStore.GetParentsByIdAsync(model.Topic.CategoryId)
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
                        model.ChannelName = sb.ToString();
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
