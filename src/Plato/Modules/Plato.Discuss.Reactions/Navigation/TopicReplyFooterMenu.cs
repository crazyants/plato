using System;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Entities.Reactions.ViewModels;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.Reactions.Navigation
{
    public class TopicReplyFooterMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }

        public TopicReplyFooterMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }
        
        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "topic-reply-footer", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Topic)] as Topic;

            if (entity == null)
            {
                return;
            }

            var reply = builder.ActionContext.HttpContext.Items[typeof(Reply)] as Reply;

            if (reply == null)
            {
                return;
            }
            
            builder
                .Add(T["React"], int.MaxValue, react => react
                    .View("ReactionList", new
                    {
                        model = new ReactionListViewModel()
                        {
                            Entity = entity,
                            Reply = reply,
                            Permission = Permissions.ReactToReplies
                        }
                    })
                    .Permission(Permissions.ViewReplyReactions)
                );

        }

    }

}
