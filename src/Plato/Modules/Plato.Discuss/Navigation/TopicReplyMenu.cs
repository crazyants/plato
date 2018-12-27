using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Internal.Navigation;

namespace Plato.Discuss.Navigation
{
    public class TopicReplyMenu : INavigationProvider
    {

        private readonly IActionContextAccessor _actionContextAccessor;
    
        public IStringLocalizer T { get; set; }

        public TopicReplyMenu(
            IStringLocalizer localizer,
            IActionContextAccessor actionContextAccessor)
        {
            T = localizer;
            _actionContextAccessor = actionContextAccessor;
        }

        public void BuildNavigation(string name, NavigationBuilder builder)
        {

            if (!String.Equals(name, "topicreply", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
        
            // Get model from navigation builder
            var reply = builder.ActionContext.HttpContext.Items[typeof(Reply)] as Reply;
            if (reply == null)
            {
                return;
            }

            // Edit reply
            builder.Add(T["Edit"], int.MinValue + 1, edit => edit
                    .IconCss("fal fa-pencil")
                    .Attributes(new Dictionary<string, object>()
                    {
                        {"data-provide", "tooltip"},
                        {"title", T["Edit"]}
                    })
                    .Action("EditReply", "Home", "Plato.Discuss", new RouteValueDictionary()
                    {
                        ["id"] = reply?.Id ?? 0
                    })
                    //.Permission(Permissions.ManageRoles)
                    .LocalNav()
                , new string[] {"edit-reply", "text-muted", "text-hidden"});


            // Options
            builder
                .Add(T["Options"], int.MaxValue, options => options
                        .IconCss("fa fa-ellipsis-h")
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-provide", "tooltip"},
                            {"title", T["Options"]}
                        })
                        .Add(T["Share"], share => share
                            .Action("Index", "Home", "Plato.Discuss")
                            .Attributes(new Dictionary<string, object>()
                            {
                                {"data-provide", "tooltip"},
                                {"title", T["Options"]}
                            })
                            //.Permission(Permissions.ManageRoles)
                            .LocalNav()
                        )
                        .Add(T["Report"], report => report
                            .Action("Popular", "Home", "Plato.Discuss")
                            //.Permission(Permissions.ManageRoles)
                            .LocalNav()
                        ), new List<string>() { "topic-options", "text-muted", "dropdown-toggle-no-caret", "text-hidden" }
                );

        }

    }

}
