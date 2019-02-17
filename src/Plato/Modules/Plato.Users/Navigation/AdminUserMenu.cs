using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Models.Users;
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

            // Get user we are editing from context
            var user = builder.ActionContext.HttpContext.Items[typeof(User)] as User;
            if (user == null)
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
                        .Add(T["Edit Password"], int.MinValue, edit => edit
                            .Action("EditPassword", "Admin", "Plato.Users", new RouteValueDictionary()
                            {
                                ["Id"] = user.Id.ToString()
                            })
                            .LocalNav()
                        )
                        .Add(T["Mark Verified"], int.MinValue, edit => edit
                            .Action("ValidateUser", "Admin", "Plato.Users", new RouteValueDictionary()
                            {
                                ["Id"] = user.Id.ToString()
                            })
                            .LocalNav()
                        )



                    , new List<string>() { "topic-options", "text-muted", "dropdown-toggle-no-caret", "text-hidden" }
                );



        }
    }

}
