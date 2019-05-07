using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Issues.Models;
using Plato.Entities.Extensions;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Issues.Navigation
{
    public class IssueMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }

        public IssueMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "issue", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from context
            var entity = builder.ActionContext.HttpContext.Items[typeof(Issue)] as Issue;
            if (entity == null)
            {
                return;
            }
            
            // Get authenticated user from context
            var user = builder.ActionContext.HttpContext.Features[typeof(User)] as User;
            
            Permission deletePermission = null;
            if (entity.IsDeleted)
            {
                // Do we have restore permissions?
                deletePermission = user?.Id == entity.CreatedUserId
                    ? Permissions.RestoreOwnIssues
                    : Permissions.RestoreAnyIssue;
            }
            else
            {
                // Do we have delete permissions?
                deletePermission = user?.Id == entity.CreatedUserId
                    ? Permissions.DeleteOwnIssues
                    : Permissions.DeleteAnyIssue;
            }
            
            builder
                .Add(T["Options"], int.MaxValue, options => options
                        .IconCss("fa fa-ellipsis-h")
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-provide", "tooltip"},
                            {"title", T["Options"]}
                        })
                        .Add(T["Edit"], int.MinValue, edit => edit
                            .Action("Edit", "Home", "Plato.Issues", new RouteValueDictionary()
                            {
                                ["opts.id"] = entity.Id,
                                ["opts.alias"] = entity.Alias
                            })
                            .Permission(user?.Id == entity.CreatedUserId
                                ? Permissions.EditOwnIssues
                                : Permissions.EditAnyIssue)
                            .LocalNav()
                        )
                        .Add(entity.IsPinned ? T["Unpin"] : T["Pin"], 1, edit => edit
                            .Action(entity.IsPinned ? "Unpin" : "Pin", "Home", "Plato.Issues",
                                new RouteValueDictionary()
                                {
                                    ["id"] = entity.Id
                                })
                            .Resource(entity.CategoryId)
                            .Permission(entity.IsPinned
                                ? Permissions.UnpinIssues
                                : Permissions.PinIssues)
                            .LocalNav()
                        )
                        .Add(entity.IsClosed ? T["Open"] : T["Close"], 2, edit => edit
                            .Action(entity.IsClosed ? "Open" : "Close", "Home", "Plato.Issues",
                                new RouteValueDictionary()
                                {
                                    ["id"] = entity.Id
                                })
                            .Resource(entity.CategoryId)
                            .Permission(entity.IsClosed
                                ? Permissions.OpenIssues
                                : Permissions.CloseIssues)
                            .LocalNav()
                        )
                        .Add(entity.IsLocked ? T["Unlock"] : T["Lock"], 2, edit => edit
                            .Action(entity.IsLocked ? "Unlock" : "Lock", "Home", "Plato.Issues",
                                new RouteValueDictionary()
                                {
                                    ["id"] = entity.Id
                                })
                            .Resource(entity.CategoryId)
                            .Permission(entity.IsLocked
                                ? Permissions.UnlockIssues
                                : Permissions.LockIssues)
                            .LocalNav()
                        )
                        .Add(entity.IsHidden ? T["Unhide"] : T["Hide"], 2, edit => edit
                            .Action(entity.IsHidden ? "Show" : "Hide", "Home", "Plato.Issues",
                                new RouteValueDictionary()
                                {
                                    ["id"] = entity.Id
                                })
                            .Resource(entity.CategoryId)
                            .Permission(entity.IsHidden
                                ? Permissions.ShowIssues
                                : Permissions.HideIssues)
                            .LocalNav()
                        )
                        .Add(entity.IsSpam ? T["Not Spam"] : T["Spam"], 2, spam => spam
                            .Action(entity.IsSpam ? "FromSpam" : "ToSpam", "Home", "Plato.Issues",
                                new RouteValueDictionary()
                                {
                                    ["id"] = entity.Id
                                })
                            .Resource(entity.CategoryId)
                            .Permission(entity.IsSpam
                                ? Permissions.IssueFromSpam
                                : Permissions.IssueToSpam)
                            .LocalNav()
                        )
                        .Add(T["Report"], int.MaxValue - 2, report => report
                            .Action("Report", "Home", "Plato.Issues", new RouteValueDictionary()
                            {
                                ["opts.id"] = entity.Id,
                                ["opts.alias"] = entity.Alias
                            })
                            .Attributes(new Dictionary<string, object>()
                            {
                                {"data-provide", "dialog"},
                                {"data-dialog-modal-css", "modal fade"},
                                {"data-dialog-css", "modal-dialog modal-lg"}
                            })
                            .Permission(Permissions.ReportIssues)
                            .LocalNav()
                        )
                        .Add(T["Divider"], int.MaxValue - 1, divider => divider
                            .Permission(deletePermission)
                            .DividerCss("dropdown-divider").LocalNav()
                        )
                        .Add(entity.IsDeleted ? T["Restore"] : T["Delete"], int.MaxValue, edit => edit
                                .Action(entity.IsDeleted ? "Restore" : "Delete", "Home", "Plato.Issues",
                                    new RouteValueDictionary()
                                    {
                                        ["id"] = entity.Id
                                    })
                                .Permission(deletePermission)
                                .LocalNav(),
                            entity.IsDeleted
                                ? new List<string>() {"dropdown-item", "dropdown-item-success"}
                                : new List<string>() {"dropdown-item", "dropdown-item-danger"}
                        )
                    , new List<string>() {"article-options", "text-muted", "dropdown-toggle-no-caret", "text-hidden"}
                );
            
            // If entity is not hidden or locked allow replies
            if (!entity.IsHidden() && !entity.IsLocked)
            {

                builder
                    .Add(T["Comment"], int.MaxValue, options => options
                            .IconCss("fa fa-reply")
                            .Attributes(new Dictionary<string, object>()
                                {
                                    {"data-provide", "postReply"},
                                    {"data-toggle", "tooltip"},
                                    {"title", T["Comment"]}
                                })
                            .Action("Login", "Account", "Plato.Users",
                                new RouteValueDictionary()
                                {
                                    ["returnUrl"] = builder.ActionContext.HttpContext.Request.Path
                                })
                            .Permission(Permissions.PostIssueComments)
                            .LocalNav()
                        , new List<string>() {"article-reply", "text-muted", "text-hidden"}
                    );

            }

        }

    }

}
















