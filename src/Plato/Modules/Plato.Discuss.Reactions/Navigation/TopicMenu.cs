using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Entities.Stores;
using Plato.Internal.Navigation;

namespace Plato.Discuss.Reactions.Navigation
{
    public class TopicMenu : INavigationProvider
    {

        private readonly IActionContextAccessor _actionContextAccessor;
    
        public IStringLocalizer T { get; set; }

        public TopicMenu(
            IStringLocalizer localizer,
            IActionContextAccessor actionContextAccessor)
        {
            T = localizer;
            _actionContextAccessor = actionContextAccessor;
        }
        
        public void BuildNavigation(string name, NavigationBuilder builder)
        {

            if (!String.Equals(name, "topic", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var context = _actionContextAccessor.ActionContext;
            var id = context.RouteData.Values["id"].ToString();
            var alias = context.RouteData.Values["alias"].ToString();
            if (!int.TryParse(id, out var entityId))
            {
                return;
            }

            builder
                .Add(T["React"], config => config
                    .View("ReactMenu", new
                    {
                        id = entityId
                    })
                );

            //builder.Add(T["React"], int.MinValue, edit => edit
            //        .IconCss("fal fa-smile")
            //        .Attributes(new Dictionary<string, object>()
            //        {
            //            {"data-provide", "tooltip"},
            //            {"title", T["React to this topic"]}
            //        })
            //        .Action("Edit", "Home", "Plato.Discuss", new RouteValueDictionary()
            //        {
            //            ["id"] = id
            //        })
            //        //.Permission(Permissions.ManageRoles)
            //        .LocalNav()
            //    , new List<string>() {"edit", "text-muted", "text-hidden"});


        }
    }
}
