using Microsoft.Extensions.Localization;
using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Navigation.Abstractions;
using System.Collections.Generic;
using Plato.Discuss.Models;

namespace Plato.Discuss.Navigation
{
    public class PostMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }
        
        public PostMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "post", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get action name
            var actionName = string.Empty;
            if (builder.ActionContext.RouteData.Values.ContainsKey("action"))
            {
                actionName = builder.ActionContext.RouteData.Values["action"].ToString();
            }

            // No need to display the menu when editing entities
            if (actionName.IndexOf("Edit", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return;
            }

            builder
                .Add(T["New"], 2, create => create
                    .IconCss("fal fa-plus")
                    .Attributes(new Dictionary<string, object>()
                    {
                        ["data-display"] = "static"
                    })
                    .Add(T["Topic"], 1, topics => topics
                        .Action("Create", "Home", "Plato.Discuss", new RouteValueDictionary())
                        .Permission(Permissions.PostTopics)
                        .LocalNav()
                    ), new List<string>() {"nav-item", "text-hidden", "text-muted" });
        }
    }

}
