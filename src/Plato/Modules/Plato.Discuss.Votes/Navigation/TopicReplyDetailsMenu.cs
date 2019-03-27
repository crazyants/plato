using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;
using Plato.Discuss.Models;
using Plato.Entities.Ratings.ViewModels;

namespace Plato.Discuss.Votes.Navigation
{
    
    public class TopicReplyDetailsMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public TopicReplyDetailsMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "topic-reply-details", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get entity from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Topic)] as Topic;
            if (entity == null)
            {
                return;
            }

            // Get reply from navigation builder
            var reply = builder.ActionContext.HttpContext.Items[typeof(Reply)] as Reply;
            if (reply == null)
            {
                return;
            }

            // Add reaction menu view to navigation
            builder
                .Add(T["Vote"], react => react
                        .View("VoteToggle", new
                        {
                            model = new VoteToggleViewModel()
                            {
                                Entity = entity,
                                Reply = reply,
                                Permission = Permissions.VoteTopicReplies,
                                ApiUrl = "api/discuss/vote/post"
                            }
                        })
                    //.Permission(Permissions.ReactToTopics)
                );

        }

    }

}
