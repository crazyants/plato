using Microsoft.Extensions.Localization;
using System;
using Plato.Internal.Navigation;

namespace Plato.Discuss.Channels.Navigation
{
    public class SiteMenu : INavigationProvider
    {
        public SiteMenu(IStringLocalizer<AdminMenu> localizer)
        {
            T = localizer;
        }

        public IStringLocalizer T { get; set; }

        public void BuildNavigation(string name, NavigationBuilder builder)
        {
            if (!String.Equals(name, "site", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            builder
                .Add(T["Discuss"], configuration => configuration
                    .Add(T["Channels"], int.MinValue, installed => installed
                        .Action("Index", "Home", "Plato.Discuss.Channels")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    )
                );

        }
    }

}
