using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Internal.Navigation;

namespace Plato.Discuss.Navigation
{
    public class TopicMenu : INavigationProvider
    {

        private readonly IActionContextAccessor _actionContextAccessor;
    
        public IStringLocalizer T { get; set; }

        public TopicMenu(
            IStringLocalizer localizer,
            IActionContextAccessor actionContextAccessor)
        {
            T = localizer;
            _actionContextAccessor = actionContextAccessor;
        }

        public void BuildNavigation(string name, NavigationBuilder builder)
        {

            if (!String.Equals(name, "topic", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            
            // Get model from navigation builder
            var topic = builder.ActionContext.HttpContext.Items[typeof(Topic)] as Topic;
            if (topic == null)
            {
                return;
            }
            
            // Edit topic
            builder.Add(T["Edit"], int.MinValue, edit => edit
                    .IconCss("fal fa-pencil")
                    .Attributes(new Dictionary<string, object>()
                    {
                        {"data-provide", "tooltip"},
                        {"title", T["Edit"]}
                    })
                    .Action("Edit", "Home", "Plato.Discuss", new RouteValueDictionary()
                    {
                        ["id"] = topic.Id
                    })
                    //.Permission(Permissions.ManageRoles)
                    .LocalNav()
                , new string[] {"edit", "text-muted", "text-hidden" });

        }

    }

}
