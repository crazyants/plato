using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Navigation;

namespace Plato.Users.Navigation
{
    public class AdminUserMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }
        
        public AdminUserMenu(IStringLocalizer<AdminMenu> localizer)
        {
            T = localizer;
        }
        
        public void BuildNavigation(string name, NavigationBuilder builder)
        {
            if (!String.Equals(name, "admin-user", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Add topic options
            builder
                .Add(T["Options"], int.MaxValue, options => options
                        .IconCss("fa fa-ellipsis-h")
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-provide", "tooltip"},
                            {"title", T["Options"]}
                        })
                        .Add(T["Update Password"], int.MinValue, edit => edit
                            .Action("Edit", "Home", "Plato.Discuss")
                            .LocalNav()
                        )
                        .Add(T["Mark As Verified"], int.MinValue, edit => edit
                            .Action("Edit", "Home", "Plato.Discuss")
                            .LocalNav()
                        )



                    , new List<string>() { "topic-options", "text-muted", "dropdown-toggle-no-caret", "text-hidden" }
                );



        }
    }

}
