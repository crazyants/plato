using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Entities.Navigation
{
    public class UserSubmissionsMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }
        
        public UserSubmissionsMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "user-submissions", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var context = builder.ActionContext;
            object id = context.RouteData.Values["opts.id"],
                alias = context.RouteData.Values["opts.alias"];

            builder.Add(T["All"], 1, topics => topics
                .Action("Display", "User", "Plato.Users", new RouteValueDictionary()
                {
                    ["opts.createdByUserId"] = id?.ToString(),
                    ["opts.alias"] = alias?.ToString()
                })
                //.Permission(Permissions.ManageRoles)
                .LocalNav(), new List<string>() {  "active" }
            );
        }
    }

}
