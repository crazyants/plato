using Microsoft.Extensions.Localization;
using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;
using System.Collections.Generic;

namespace Plato.Questions.Navigation
{
    public class PostMenu : INavigationProvider
    {

        private readonly IActionContextAccessor _actionContextAccessor;

        public IStringLocalizer T { get; set; }
        
        public PostMenu(
            IStringLocalizer localizer, 
            IActionContextAccessor actionContextAccessor)
        {
            T = localizer;
            _actionContextAccessor = actionContextAccessor;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "post", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            builder
                .Add(T["New"], 1, create => create
                    .IconCss("fal fa-plus")
                    .Attributes(new Dictionary<string, object>()
                    {
                        ["data-display"] = "static"
                    })
                    .Add(T["Question"], 4, question => question
                        .Action("Create", "Home", "Plato.Questions", new RouteValueDictionary())
                        .Permission(Permissions.PostQuestions)
                        .LocalNav()
                    ), new List<string>() {"nav-item", "text-hidden", "active" });
        }
    }

}
