using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Articles.Models;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Articles.Navigation
{
    public class ArticleMenu : INavigationProvider
    {

        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IContextFacade _contextFacade;

        public IStringLocalizer T { get; set; }

        public ArticleMenu(
            IStringLocalizer localizer,
            IActionContextAccessor actionContextAccessor,
            IContextFacade contextFacade)
        {
            T = localizer;
            _actionContextAccessor = actionContextAccessor;
            _contextFacade = contextFacade;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "article", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from context
            var topic = builder.ActionContext.HttpContext.Items[typeof(Article)] as Article;
            if (topic == null)
            {
                return;
            }
            
            // Get authenticated user from context
            var user = builder.ActionContext.HttpContext.Features[typeof(User)] as User;
            
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
                    ? Permissions.DeleteArticles
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
                            .Action("Edit", "Home", "Plato.Articles", new RouteValueDictionary()
                            {
                                ["id"] = topic.Id,
                                ["alias"] = topic.Alias
                            })
                            .Permission(user?.Id == topic.CreatedUserId
                                ? Permissions.EditArticles
                                : Permissions.EditAnyTopic)
                            .LocalNav()
                        )
                        .Add(T["Report"], int.MaxValue - 2, report => report
                            .Action("Report", "Home", "Plato.Articles")
                            .Attributes(new Dictionary<string, object>()
                            {
                                {"data-provide", "dialog"},
                                {"data-dialog-modal-css", "modal fade"},
                                {"data-dialog-css", "modal-dialog modal-lg"}
                            })
                            .Permission(Permissions.ReportTopics)
                            .LocalNav()
                        )
                        .Add(T["Divider"], int.MaxValue - 1, divider => divider
                            .Permission(deletePermission)
                            .DividerCss("dropdown-divider").LocalNav()
                        )
                        .Add(topic.IsDeleted ? T["Restore"] : T["Delete"], int.MaxValue, edit => edit
                                .Action(topic.IsDeleted ? "RestoreTopic" : "DeleteTopic", "Home", "Plato.Articles",
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
                            .Action("Login", "Account", "Plato.Users",
                                new RouteValueDictionary()
                                {
                                    ["returnUrl"] = builder.ActionContext.HttpContext.Request.Path
                                })
                            .Permission(Permissions.PostComments)
                            .LocalNav()
                        , new List<string>() {"topic-reply", "text-muted", "text-hidden"}
                    );

            }

        }

    }

}
