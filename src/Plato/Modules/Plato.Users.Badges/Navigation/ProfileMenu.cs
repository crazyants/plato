using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Navigation;

namespace Plato.Users.Badges.Navigation
{
    public class ProfileMenu : INavigationProvider
    {

        private readonly IActionContextAccessor _actionContextAccessor;

        public IStringLocalizer T { get; set; }
        
        public ProfileMenu(
            IStringLocalizer localizer, 
            IActionContextAccessor actionContextAccessor)
        {
            T = localizer;
            _actionContextAccessor = actionContextAccessor;
        }
        
        public void BuildNavigation(string name, NavigationBuilder builder)
        {
            if (!String.Equals(name, "profile", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var context = _actionContextAccessor.ActionContext;
            object id = context.RouteData.Values["id"], 
                alias = context.RouteData.Values["alias"];

            builder.Add(T["Badges"], 1, topics => topics
                .Action("Index", "Home", "Plato.Users.Badges", new RouteValueDictionary()
                {
                    ["id"] = id?.ToString(),
                    ["alias"] = alias?.ToString()
                })
                //.Permission(Permissions.ManageRoles)
                .LocalNav()
            );
        }
    }

}
