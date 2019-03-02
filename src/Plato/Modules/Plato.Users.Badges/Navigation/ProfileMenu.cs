using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Users.Badges.Navigation
{
    public class ProfileMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }
        
        public ProfileMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }
        
        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            // Ensure correct provider
            if (!String.Equals(name, "profile", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get user from context
            var user = builder.ActionContext.HttpContext.Items[typeof(User)] as User;
            if (user == null)
            {
                return;
            }
            
            // Build menu
            builder.Add(T["Badges"], 1, topics => topics
                .Action("Index", "Profile", "Plato.Users.Badges", new RouteValueDictionary()
                {
                    ["opts.id"] = user.Id.ToString(),
                    ["opts.alias"] = user.Alias.ToString()
                })
                //.Permission(Permissions.ManageRoles)
                .LocalNav()
            );
        }
    }

}
