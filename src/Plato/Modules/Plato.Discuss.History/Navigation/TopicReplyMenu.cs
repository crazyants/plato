using System;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Entities.History.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.History.Navigation
{

    public class TopicReplyMenu : INavigationProvider
    {

        private readonly IContextFacade _contextFacade;

        public IStringLocalizer T { get; set; }

        public TopicReplyMenu(
            IStringLocalizer localizer,
             IContextFacade contextFacade)
        {
            _contextFacade = contextFacade;
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

            if (reply == null)
            {
                return;
            }

            // No edited information
            if (reply.EditedDate == null)
            {
                return;
            }

            // Add HistoryMenu view to reply
            builder
                .Add(T["History"], int.MinValue, history => history
                    .View("HistoryMenu", new
                    {
                        model = new HistoryMenuViewModel()
                        {
                            Entity = entity,
                            Reply = reply,
                            DialogUrl = _contextFacade.GetRouteUrl(new RouteValueDictionary()
                            {
                                ["area"] = "Plato.Discuss.History",
                                ["controller"] = "Home",
                                ["action"] = "Index",
                                ["id"] = 0
                            })
                        }
                    })
                    .Permission(Permissions.viewReplyHistory)
                );
            
        }

    }

}
