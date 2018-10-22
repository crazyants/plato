using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Layout.Views;
using Plato.Internal.Navigation;

namespace Plato.Discuss.Navigation
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

            //builder
            //    .Add(T["Edit By View"], config => config
            //        .View("TopicMenu", new
            //        {
            //            id = entityId
            //        })
            //    );

            builder.Add(T["Edit Topic"], int.MinValue, edit => edit
                    .IconCss("fal fa-pencil")
                    .Attributes(new Dictionary<string, object>()
                    {
                        {"data-provide", "tooltip"},
                        {"title", T["Edit Topic"]}
                    })
                    .Action("Edit", "Home", "Plato.Discuss", new RouteValueDictionary()
                    {
                        ["id"] = id
                    })
                    //.Permission(Permissions.ManageRoles)
                    .LocalNav()
                , new List<string>() {"edit", "text-muted", "text-hidden"});

            //builder
            //    .Add(T["Discuss"], configuration => configuration
            //            .IconCss("fal fa-comment-alt fa-flip-y")
            //            .Attributes(new Dictionary<string, object>()
            //            {
            //                {"data-provide", "tooltip"},
            //                {"title", T["Discuss"]}
            //            })
            //            .Add(T["Latest"], int.MinValue, installed => installed
            //                .View("TopicMenu", new
            //                {
            //                    id = entityId
            //                })
            //                //.Permission(Permissions.ManageRoles)
            //                .LocalNav()
            //            )
            //            .Add(T["Popular"], int.MinValue + 1, installed => installed
            //                .Action("Popular", "Home", "Plato.Discuss")
            //                //.Permission(Permissions.ManageRoles)
            //                .LocalNav()
            //            ), new List<string>() {"discuss", "text-muted", "text-hidden"}
            //    );

            //builder
            //    .Add(T["Test Menu"], configuration => configuration
            //            .IconCss("fal fa-plus")
            //            .Attributes(new Dictionary<string, object>()
            //            {
            //                {"data-provide", "tooltip"},
            //                {"title", T["Discuss"]}
            //            })
            //            .Add(T["Latest"], int.MinValue, installed => installed
            //                .View("TopicMenu", new
            //                {
            //                    id = entityId
            //                })
            //                //.Permission(Permissions.ManageRoles)
            //                .LocalNav()
            //            )
            //            .Add(T["Popular"], int.MinValue + 1, installed => installed
            //                .Action("Popular", "Home", "Plato.Discuss")
            //                //.Permission(Permissions.ManageRoles)
            //                .LocalNav()
            //            ), new List<string>() { "discuss", "text-muted", "text-hidden" }
            //    );


        }
    }
}
