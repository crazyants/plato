using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Entities.Extensions;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Discuss.Navigation
{
    public class TopicMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public TopicMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
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
                                ["opts.id"] = topic.Id,
                                ["opts.alias"] = topic.Alias
                            })
                            .Permission(user?.Id == topic.CreatedUserId
                                ? Permissions.EditOwnTopics
                                : Permissions.EditAnyTopic)
                            .LocalNav()
                        )
                        .Add(topic.IsPinned ? T["Unpin"] : T["Pin"], 1, edit => edit
                            .Action(topic.IsPinned ? "Unpin" : "Pin", "Home", "Plato.Discuss",
                                new RouteValueDictionary()
                                {
                                    ["id"] = topic.Id
                                })
                            .Resource(topic.CategoryId)
                            .Permission(topic.IsPinned
                                ? Permissions.UnpinTopics
                                : Permissions.PinTopics)
                            .LocalNav()
                        )
                        .Add(topic.IsLocked ? T["Unlock"] : T["Lock"], 2, edit => edit
                            .Action(topic.IsLocked ? "Unlock" : "Lock", "Home", "Plato.Discuss",
                                new RouteValueDictionary()
                                {
                                    ["id"] = topic.Id
                                })
                            .Resource(topic.CategoryId)
                            .Permission(topic.IsLocked
                                ? Permissions.UnlockTopics
                                : Permissions.LockTopics)
                            .LocalNav()
                        )
                        .Add(topic.IsPrivate ? T["Unhide"] : T["Hide"], 2, edit => edit
                            .Action(topic.IsPrivate ? "Show" : "Hide", "Home", "Plato.Discuss",
                                new RouteValueDictionary()
                                {
                                    ["id"] = topic.Id
                                })
                            .Resource(topic.CategoryId)
                            .Permission(topic.IsPrivate
                                ? Permissions.ShowTopics
                                : Permissions.HideTopics)
                            .LocalNav()
                        )
                        .Add(topic.IsSpam ? T["Not Spam"] : T["Spam"], 2, spam => spam
                            .Action(topic.IsSpam ? "FromSpam" : "ToSpam", "Home", "Plato.Discuss",
                                new RouteValueDictionary()
                                {
                                    ["id"] = topic.Id
                                })
                            .Resource(topic.CategoryId)
                            .Permission(topic.IsSpam
                                ? Permissions.TopicFromSpam
                                : Permissions.TopicToSpam)
                            .LocalNav()
                        )
                        .Add(T["Report"], int.MaxValue - 2, report => report
                            .Action("Report", "Home", "Plato.Discuss", new RouteValueDictionary()
                            {
                                ["opts.id"] = topic.Id,
                                ["opts.alias"] = topic.Alias
                            })
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
                                .Action(topic.IsDeleted ? "Restore" : "Delete", "Home", "Plato.Discuss",
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

            // If entity is not hidden or locked allow replies
            if (!topic.IsHidden() && !topic.IsLocked)
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
                            .Permission(Permissions.PostReplies)
                            .LocalNav()
                        , new List<string>() {"topic-reply", "text-muted", "text-hidden"}
                    );

            }

        }

    }

}
