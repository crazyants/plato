using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Users.Navigation
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
        
        public void BuildNavigation(string name, INavigationBuilder builder)
        {
            if (!String.Equals(name, "profile", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var context = _actionContextAccessor.ActionContext;
            object id = context.RouteData.Values["opts.id"],
                alias = context.RouteData.Values["opts.alias"];

            builder
                .Add(T["Overview"], 1, profile => profile
                    .Action("Display", "Home", "Plato.Users", new RouteValueDictionary()
                {
                    ["opts.id"] = id?.ToString(),
                    ["opts.alias"] = alias?.ToString()
                })
                    //.Permission(Permissions.ManageUsers)
                    .LocalNav()
                );

        }
    }

}
