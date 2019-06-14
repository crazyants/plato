using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Plato.Entities.Models;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.Navigation
{
    public class HomeMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public HomeMenu(IStringLocalizer localizer,
            IHttpContextAccessor httpContextAccessor)
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
                builder.ActionContext.HttpContext.Items[typeof(FeatureEntityMetrics)] as
                    FeatureEntityMetrics;
            
            builder
                .Add(T["Discuss"], 1, discuss => discuss
                    .View("CoreDiscussMenu", model)
                    //.Permission(Permissions.ManageRoles)
                    .LocalNav()
                );

        }

    }

}
