using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Security.Abstractions;

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
            
            // Get user from context
            var user = builder.ActionContext.HttpContext.Items[typeof(User)] as User;
            
            Permission deletePermission = null;
            if (topic.IsDeleted)
            {
                // Do we have restore permissions?
                deletePermission = user?.Id == topic.CreatedUserId
                    ? Permissions.RestoreOwnTopics
                    : Permissions.RestoreAnyTopic;
            }
            else
            {
                // Do we have delete permissions?
                deletePermission = user?.Id == topic.CreatedUserId
                    ? Permissions.DeleteOwnTopics
                    : Permissions.DeleteAnyTopic;
            }

            // Add topic options
            builder
                .Add(T["Options"], int.MaxValue, options => options
                        .IconCss("fa fa-ellipsis-h")
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-provide", "tooltip"},
                            {"title", T["Options"]}
                        })
                        .Add(T["Edit"], int.MinValue, edit => edit
                            .Action("Edit", "Home", "Plato.Discuss", new RouteValueDictionary()
                            {
                                ["id"] = topic.Id,
                                ["alias"] = topic.Alias
                            })
                            .Permission(user?.Id == topic.CreatedUserId
                                ? Permissions.EditOwnTopics
                                : Permissions.EditAnyTopic)
                            .LocalNav()
                        )
                        .Add(T["Report"], int.MaxValue - 2, report => report
                            .Action("Report", "Home", "Plato.Discuss")
                            .Attributes(new Dictionary<string, object>()
                            {
                                {"data-toggle", "dialog"}
                            })
                            .Permission(Permissions.ReportTopics)
                            .LocalNav()
                        )
                        .Add(T["Divider"], int.MaxValue - 1, divider => divider
                            .Permission(user?.Id == topic.CreatedUserId
                                ? Permissions.DeleteOwnTopics
                                : Permissions.DeleteAnyTopic)
                            .DividerCss("dropdown-divider").LocalNav()
                        )
                        .Add(topic.IsDeleted ? T["Restore"] : T["Delete"], int.MaxValue, edit => edit
                                .Action(topic.IsDeleted ? "RestoreTopic" : "DeleteTopic", "Home", "Plato.Discuss",
                                    new RouteValueDictionary()
                                    {
                                        ["id"] = topic.Id
                                    })
                                .Permission(deletePermission)
                                .LocalNav(),
                            topic.IsDeleted
                                ? new List<string>() {"dropdown-item", "dropdown-item-success"}
                                : new List<string>() {"dropdown-item", "dropdown-item-danger"}
                        )
                    , new List<string>() {"topic-options", "text-muted", "dropdown-toggle-no-caret", "text-hidden"}
                );

            if (!topic.IsClosed)
            {
                builder
                    .Add(T["Reply"], int.MaxValue, options => options
                            .IconCss("fa fa-reply")
                            .Action("Login", "Account", "Plato.Users",
                                new RouteValueDictionary()
                                {
                                    ["returnUrl"] = builder.ActionContext.HttpContext.Request.Path
                                })
                            .Attributes(user == null
                                ? new Dictionary<string, object>()
                                {
                                    {"data-toggle", "tooltip"},
                                    {"title", T["Login to Reply"]}
                                }
                                : new Dictionary<string, object>()
                                {
                                    {"data-provide", "postReply"},
                                    {"data-toggle", "tooltip"},
                                    {"title", T["Reply"]}
                                })
                        , new List<string>() {"topic-reply", "text-muted", "text-hidden"}
                    );

            }

        }

    }

}
