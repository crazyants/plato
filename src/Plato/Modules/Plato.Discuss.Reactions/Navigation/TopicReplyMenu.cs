using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Entities.Reactions.ViewModels;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.Reactions.Navigation
{
    public class TopicReplyMenu : INavigationProvider
    {

        private readonly IActionContextAccessor _actionContextAccessor;
    
        public IStringLocalizer T { get; set; }

        public TopicReplyMenu(
            IStringLocalizer localizer,
            IActionContextAccessor actionContextAccessor)
        {
            T = localizer;
            _actionContextAccessor = actionContextAccessor;
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
            
            // Add reaction menu view to navigation
            builder
                .Add(T["React"], react => react
                    .View("ReactionMenu", new
                    {
                        model = new ReactionMenuViewModel()
                        {
                            ModuleId = "Plato.Discuss.Reactions",
                            Entity = entity,
                            Reply = reply
                        }
                    })
                    .Permission(Permissions.ReactToReplies)
                );
            
        }

    }

}
