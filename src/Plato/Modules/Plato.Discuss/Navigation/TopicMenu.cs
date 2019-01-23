using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Internal.Models.Users;
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

            // Get model from context
            var topic = builder.ActionContext.HttpContext.Items[typeof(Topic)] as Topic;
            if (topic == null)
            {
                return;
            }

            // Build options menu
            builder
                .Add(T["Options"], int.MaxValue, options => options
                        .IconCss("fa fa-ellipsis-h")
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-provide", "tooltip"},
                            {"title", T["Options"]}
                        })
                        .Add(T["Report"], int.MaxValue, report => report
                            .Action("Report", "Home", "Plato.Discuss")
                            .Attributes(new Dictionary<string, object>()
                            {
                                {"data-toggle", "dialog"}
                            })
                            //.Permission(user.Id == topic.Id ?
                            //    Permissions.EditOwnTopics :
                            //    Permissions.EditAnyTopic)
                            .LocalNav()
                        ), new List<string>() { "topic-options", "text-muted", "dropdown-toggle-no-caret", "text-hidden" }
                );


            // Get user from context
            var user = builder.ActionContext.HttpContext.Items[typeof(User)] as User;
            if (user == null)
            {
                return;
            }
            
            // Add edit topic option
            builder
                .Add(T["Options"], int.MaxValue, options => options
                        .Add(T["Edit"], int.MinValue, edit => edit
                            .Action("Edit", "Home", "Plato.Discuss", new RouteValueDictionary()
                            {
                                ["id"] = topic.Id,
                                ["alias"] = topic.Alias
                            })
                            .Permission(user.Id == topic.CreatedUserId ?
                                Permissions.EditOwnTopics :
                                Permissions.EditAnyTopic)
                            .LocalNav()
                        ), new List<string>() {"topic-options", "text-muted", "dropdown-toggle-no-caret", "text-hidden"}
                );




        }

    }

}
