using System;
using System.Linq;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Routing;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Questions.Navigation
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

            // Get feature metrics
            var metric = model?.Metrics?.FirstOrDefault(m => m.ModuleId.Equals("Plato.Ideas", StringComparison.OrdinalIgnoreCase));

            // Get route values
            var context = builder.ActionContext;
            object id = context.RouteData.Values["opts.createdByUserId"], 
                alias = context.RouteData.Values["opts.alias"];

            builder.Add(T["Questions"], 4, topics => topics
                .Badge(metric != null ? metric.Count.ToPrettyInt() : string.Empty, "badge badge-primary float-right")
                .Action("Index", "User", "Plato.Questions", new RouteValueDictionary()
                {
                    ["opts.createdByUserId"] = id?.ToString(),
                    ["opts.alias"] = alias?.ToString()
                })
                //.Permission(Permissions.ManageRoles)
                .LocalNav()
            );

        }

    }

}
