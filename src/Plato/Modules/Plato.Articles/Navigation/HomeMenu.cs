using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Localization;
using Plato.Entities.Models;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Articles.Navigation
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
                .Add(T["Articles"], 3, discuss => discuss
                    .View("CoreArticlesMenu", model)
                    //.Permission(Permissions.ManageRoles)
                    .LocalNav()
                );

        }
    }

}
