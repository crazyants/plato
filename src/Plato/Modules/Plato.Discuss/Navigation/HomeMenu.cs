using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.Navigation
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

            builder
                .Add(T["Discuss"], 1, discuss => discuss
                    .View("CoreDiscussMenu")
                    //.Permission(Permissions.ManageRoles)
                    .LocalNav()
                );

        }
    }

}
