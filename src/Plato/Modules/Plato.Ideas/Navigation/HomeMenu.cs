using System;
using Microsoft.Extensions.Localization;
using Plato.Entities.Models;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Ideas.Navigation
{
    public class HomeMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public HomeMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "home", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get metrics from context, these are registered via the
            // HomeMenuContextualize action filter within Plato.Entities
            var model =
                builder.ActionContext.HttpContext.Items[typeof(FeatureEntityCounts)] as
                    FeatureEntityCounts;
            
            builder
                .Add(T["Ideas"], 5, docs => docs
                    .View("CoreIdeasMenu", model)
                    //.Permission(Permissions.ManageRoles)
                    .LocalNav()
                );

        }
    }

}
