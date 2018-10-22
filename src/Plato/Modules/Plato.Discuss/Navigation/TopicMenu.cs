using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.Localization;
using Plato.Entities.Models;
using Plato.Internal.Layout.Views;
using Plato.Internal.Navigation;

namespace Plato.Discuss.Navigation
{
    public class TopicMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }

        public TopicMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }
        
        public void BuildNavigation(string name, NavigationBuilder builder)
        {

            if (!String.Equals(name, "topic", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            builder
                .Add(T["Discuss"], configuration => configuration
                        .IconCss("fal fa-comment-alt fa-flip-y")
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-provide", "tooltip"},
                            {"title", T["Discuss"]}
                        })
                        .Add(T["Latest"], int.MinValue, installed => installed
                            .View("TopicMenu", new
                            {
                                entity = new Entity()
                            })
                            //.Permission(Permissions.ManageRoles)
                            .LocalNav()
                        )
                        .Add(T["Popular"], int.MinValue + 1, installed => installed
                            .Action("Popular", "Home", "Plato.Discuss")
                            //.Permission(Permissions.ManageRoles)
                            .LocalNav()
                        ), new List<string>() {"discuss", "text-muted", "text-hidden"}
                );

            builder
                .Add(T["Test Menu"], configuration => configuration
                        .IconCss("fal fa-plus")
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-provide", "tooltip"},
                            {"title", T["Discuss"]}
                        })
                        .Add(T["Latest"], int.MinValue, installed => installed
                            .View("TopicMenu", new
                            {
                                entity = new Entity()
                            })
                            //.Permission(Permissions.ManageRoles)
                            .LocalNav()
                        )
                        .Add(T["Popular"], int.MinValue + 1, installed => installed
                            .Action("Popular", "Home", "Plato.Discuss")
                            //.Permission(Permissions.ManageRoles)
                            .LocalNav()
                        ), new List<string>() { "discuss", "text-muted", "text-hidden" }
                );


        }
    }
}
