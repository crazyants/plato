using System;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Entities.Extensions;
using Plato.Entities.Reactions.ViewModels;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.Reactions.Navigation
{
    public class TopicReplyMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }

        public TopicReplyMenu(IStringLocalizer localizer)
        {
            T = localizer;            
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "topic-reply", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Topic)] as Topic;
            var reply = builder.ActionContext.HttpContext.Items[typeof(Reply)] as Reply;

            // We need an entity
            if (entity == null)
            {
                return;
            }

            // We need a reply
            if (reply == null)
            {
                return;
            }
            
            // No need to show reactions if entity is hidden
            if (entity.IsHidden())
            {
                return;
            }

            // No need to show reactions if reply is hidden
            if (reply.IsHidden())
            {
                return;
            }

            // Add reaction menu view to navigation
            builder
                .Add(T["React"], react => react
                    .View("ReactionMenu", new
                    {
                        model = new ReactionMenuViewModel()
                        {
                            ModuleId = "Plato.Discuss.Reactions",
                            Entity = entity,
                            Reply = reply,
                            Permission = Permissions.ReactToReplies
                        }
                    })                    
                );

        }

    }

}
