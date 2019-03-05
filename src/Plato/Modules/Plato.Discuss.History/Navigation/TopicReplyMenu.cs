using System;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.History.Navigation
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
            var topic = builder.ActionContext.HttpContext.Items[typeof(Topic)] as Topic;
            var reply = builder.ActionContext.HttpContext.Items[typeof(Reply)] as Reply;

            // Add HistoryMenu view to reply
            builder
                .Add(T["History"], int.MinValue, history => history
                    .View("HistoryMenu", new
                    {
                        topic,
                        reply
                    })
                    .Permission(Permissions.ViewReplyHistory)
                );
            
        }

    }

}
