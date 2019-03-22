using Microsoft.Extensions.Localization;
using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Questions.Navigation
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
                .Add(T["Questions"], 3, discuss => discuss
                    .Add(T["Topics"], 1, topics => topics
                        .Action("Index", "Profile", "Plato.Questions", new RouteValueDictionary()
                        {
                            ["opts.id"] = id?.ToString(),
                            ["opts.alias"] = alias?.ToString()
                        })
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ).Add(T["Favorites"], 999, favorites => favorites
                        .Action("Channels", "Admin", "Plato.Discuss")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ));
        }
    }

}
