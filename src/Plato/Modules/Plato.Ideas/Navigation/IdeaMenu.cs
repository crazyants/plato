using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Entities.Extensions;
using Plato.Ideas.Models;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Ideas.Navigation
{
    public class IdeaMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }

        public IdeaMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "idea", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from context
            var entity = builder.ActionContext.HttpContext.Items[typeof(Idea)] as Idea;
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
                    ? Permissions.RestoreOwnIdeas
                    : Permissions.RestoreAnyIdea;
            }
            else
            {
                // Do we have delete permissions?
                deletePermission = user?.Id == entity.CreatedUserId
                    ? Permissions.DeleteOwnIdeas
                    : Permissions.DeleteAnyIdeaComment;
            }

            // Add entity options
            builder
                .Add(T["Options"], int.MaxValue, options => options
                        .IconCss("fa fa-ellipsis-h")
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-provide", "tooltip"},
                            {"title", T["Options"]}
                        })
                        .Add(T["Edit"], int.MinValue, edit => edit
                            .Action("Edit", "Home", "Plato.Ideas", new RouteValueDictionary()
                            {
                                ["opts.id"] = entity.Id,
                                ["opts.alias"] = entity.Alias
                            })
                            .Permission(user?.Id == entity.CreatedUserId
                                ? Permissions.EditOwnIdeas
                                : Permissions.EditAnyIdea)
                            .LocalNav()
                        )
                            .Add(entity.IsPinned ? T["Unpin"] : T["Pin"], 1, edit => edit
                            .Action(entity.IsPinned ? "Unpin" : "Pin", "Home", "Plato.Ideas",
                                new RouteValueDictionary()
                                {
                                    ["id"] = entity.Id
                                })
                            .Resource(entity.CategoryId)
                            .Permission(entity.IsPinned
                                ? Permissions.UnpinIdeas
                                : Permissions.PinIdeas)
                            .LocalNav()
                        )
                        .Add(entity.IsLocked ? T["Unlock"] : T["Lock"], 2, edit => edit
                            .Action(entity.IsLocked ? "Unlock" : "Lock", "Home", "Plato.Ideas",
                                new RouteValueDictionary()
                                {
                                    ["id"] = entity.Id
                                })
                            .Resource(entity.CategoryId)
                            .Permission(entity.IsLocked
                                ? Permissions.UnlockIdeas
                                : Permissions.LockIdeas)
                            .LocalNav()
                        )
                        .Add(entity.IsHidden ? T["Unhide"] : T["Hide"], 2, edit => edit
                            .Action(entity.IsHidden ? "Show" : "Hide", "Home", "Plato.Ideas",
                                new RouteValueDictionary()
                                {
                                    ["id"] = entity.Id
                                })
                            .Resource(entity.CategoryId)
                            .Permission(entity.IsHidden
                                ? Permissions.ShowIdeas
                                : Permissions.HideIdeas)
                            .LocalNav()
                        )
                        .Add(entity.IsSpam ? T["Not Spam"] : T["Spam"], 2, spam => spam
                            .Action(entity.IsSpam ? "FromSpam" : "ToSpam", "Home", "Plato.Ideas",
                                new RouteValueDictionary()
                                {
                                    ["id"] = entity.Id
                                })
                            .Resource(entity.CategoryId)
                            .Permission(entity.IsSpam
                                ? Permissions.IdeaFromSpam
                                : Permissions.IdeaToSpam)
                            .LocalNav()
                        )
                        .Add(T["Report"], int.MaxValue - 2, report => report
                            .Action("Report", "Home", "Plato.Ideas", new RouteValueDictionary()
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
                            .Permission(Permissions.ReportIdeas)
                            .LocalNav()
                        )
                        .Add(T["Divider"], int.MaxValue - 1, divider => divider
                            .Permission(deletePermission)
                            .DividerCss("dropdown-divider").LocalNav()
                        )
                        .Add(entity.IsDeleted ? T["Restore"] : T["Delete"], int.MaxValue, edit => edit
                                .Action(entity.IsDeleted ? "Restore" : "Delete", "Home", "Plato.Ideas",
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
                    , new List<string>() {"idea-options", "text-muted", "dropdown-toggle-no-caret", "text-hidden"}
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
                            .Permission(Permissions.PostIdeaComments)
                            .LocalNav()
                        , new List<string>() {"idea-reply", "text-muted", "text-hidden"}
                    );

            }

        }

    }

}
