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

            // Edit reply
            builder.Add(T["Edit"], int.MinValue, edit => edit
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
        }

    }

}
