using System;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Entities.History.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.History.Navigation
{

    public class TopicMenu : INavigationProvider
    {

        private readonly IContextFacade _contextFacade;

        public IStringLocalizer T { get; set; }

        public TopicMenu(
            IStringLocalizer localizer,
             IContextFacade contextFacade)
        {
            _contextFacade = contextFacade;
            T = localizer;
        }
        
        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "topic", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Topic)] as Topic;
            if (entity == null)
            {
                return;
            }

            // No edited information
            if (entity.EditedDate == null)
            {
                return;
            }

            // Add HistoryMenu view to entity
            builder
                .Add(T["History"], int.MinValue, history => history
                    .View("HistoryMenu", new
                    {
                        model = new HistoryMenuViewModel()
                        {
                            Entity = entity,
                            DialogUrl = _contextFacade.GetRouteUrl(new RouteValueDictionary()
                            {
                                ["area"] = "Plato.Discuss.History",
                                ["controller"] = "Home",
                                ["action"] = "Index",
                                ["id"] = 0
                            })
                        }
                    })
                    .Permission(Permissions.ViewEntityHistory)
                );

        }

    }

}
