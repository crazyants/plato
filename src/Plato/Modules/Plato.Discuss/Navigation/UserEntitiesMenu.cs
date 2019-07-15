using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Entities.Models;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.Navigation
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
                builder.ActionContext.HttpContext.Items[typeof(FeatureEntityCounts)] as
                    FeatureEntityCounts;

            // Current area name
            var areaName = "Plato.Discuss";

            // Get feature metrics
            var metric = model?.Features?.FirstOrDefault(m => m.ModuleId.Equals(areaName, StringComparison.OrdinalIgnoreCase));
            
            // Get route values
            var context = builder.ActionContext;
            object id = context.RouteData.Values["opts.createdByUserId"],
                alias = context.RouteData.Values["opts.alias"];
            var isArea = context.RouteData.Values["area"].ToString()
                .Equals(areaName, StringComparison.OrdinalIgnoreCase);
            var isController = context.RouteData.Values["controller"].ToString()
                .Equals("User", StringComparison.OrdinalIgnoreCase);
            var isAction = context.RouteData.Values["action"].ToString()
                .Equals("Index", StringComparison.OrdinalIgnoreCase);

            var css = "";
            if (isArea && isController && isAction)
            {
                css = "active";
            }

            builder.Add(T["Topics"], 1, topics => topics
                .Badge(metric != null ? metric.Count.ToPrettyInt() : string.Empty, "badge badge-primary float-right")
                .Action("Index", "User", "Plato.Discuss", new RouteValueDictionary()
                {
                    ["opts.createdByUserId"] = id?.ToString(),
                    ["opts.alias"] = alias?.ToString()
                })
                //.Permission(Permissions.ManageRoles)
                .LocalNav(), new List<string>() { css }
            );
        }
    }

}
