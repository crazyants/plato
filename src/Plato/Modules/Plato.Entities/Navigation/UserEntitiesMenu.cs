using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Entities.Navigation
{
    public class UserEntitiesMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }
        
        public UserEntitiesMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "user-entities", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get metrics from context
            var model =
                builder.ActionContext.HttpContext.Items[typeof(FeatureEntityMetrics)] as
                    FeatureEntityMetrics;

            var total = model?.Total ?? 0;

            // Get route values
            var context = builder.ActionContext;
            object id = context.RouteData.Values["opts.id"],
                alias = context.RouteData.Values["opts.alias"];

            builder.Add(T["All"], 1, topics => topics
                    .Badge(total > 0 ? total.ToPrettyInt() : string.Empty, "badge badge-primary ml-2")
                    .Action("Display", "User", "Plato.Users", new RouteValueDictionary()
                    {
                        ["opts.createdByUserId"] = id?.ToString(),
                        ["opts.alias"] = alias?.ToString()
                    })
                    //.Permission(Permissions.ManageRoles)
                    .LocalNav(), new List<string>() {"active"}
            );

        }

    }

}
