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

    public class IdeaCommentMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public IdeaCommentMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "idea-comment", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get entity from context
            var entity = builder.ActionContext.HttpContext.Items[typeof(Idea)] as Idea;
            if (entity == null)
            {
                return;
            }
            
            // Get reply from context
            var reply = builder.ActionContext.HttpContext.Items[typeof(IdeaComment)] as IdeaComment;
            if (reply == null)
            {
                return;
            }

            //// Get authenticated user from features
            var user = builder.ActionContext.HttpContext.Features[typeof(User)] as User;

            // Get delete / restore permission
            Permission deletePermission = null;
            if (reply.IsDeleted)
            {
                deletePermission = user?.Id == reply.CreatedUserId
                    ? Permissions.RestoreOwnIdeaComments
                    : Permissions.RestoreAnyIdeaComment;
            }
            else
            {
                deletePermission = user?.Id == reply.CreatedUserId
                    ? Permissions.DeleteOwnIdeaComments
                    : Permissions.DeleteAnyIdeaComment;
            }
            
            // Options
            builder
                .Add(T["Options"], int.MaxValue, options => options
                        .IconCss("fa fa-ellipsis-h")
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-provide", "tooltip"},
                            {"title", T["Options"]}
                        })
                        .Add(T["Edit"], int.MinValue, edit => edit
                            .Action("EditReply", "Home", "Plato.Ideas", new RouteValueDictionary()
                            {
                                ["id"] = reply?.Id ?? 0
                            })
                            .Permission(user?.Id == reply.CreatedUserId ?
                                Permissions.EditOwnIdeaComments :
                                Permissions.EditAnyIdeaComment)
                            .LocalNav())
                        .Add(reply.IsPrivate ? T["Unhide"] : T["Hide"], 2, edit => edit
                            .Action(reply.IsPrivate ? "ShowReply" : "HideReply", "Home", "Plato.Ideas",
                                new RouteValueDictionary()
                                {
                                    ["id"] = reply?.Id ?? 0
                                })
                            .Resource(entity.CategoryId)
                            .Permission(reply.IsPrivate
                                ? Permissions.ShowIdeaComments
                                : Permissions.HideIdeaComments)
                            .LocalNav()
                        )
                        .Add(reply.IsSpam ? T["Not Spam"] : T["Spam"], 3, spam => spam
                            .Action(reply.IsSpam ? "ReplyFromSpam" : "ReplyToSpam", "Home", "Plato.Ideas",
                                new RouteValueDictionary()
                                {
                                    ["id"] = reply?.Id ?? 0
                                })
                            .Resource(entity.CategoryId)
                            .Permission(reply.IsSpam
                                ? Permissions.IdeaCommentFromSpam
                                : Permissions.IdeaCommentToSpam)
                            .LocalNav()
                        )
                        .Add(T["Report"], int.MaxValue - 2, report => report
                            .Action("Report", "Home", "Plato.Ideas", new RouteValueDictionary()
                            {
                                ["opts.id"] = entity.Id,
                                ["opts.alias"] = entity.Alias,
                                ["opts.replyId"] = reply.Id
                            })
                            .Attributes(new Dictionary<string, object>()
                            {
                                {"data-provide", "dialog"},
                                {"data-dialog-modal-css", "modal fade"},
                                {"data-dialog-css", "modal-dialog modal-lg"}
                            })
                            .Permission(Permissions.ReportIdeaComments)
                            .LocalNav()
                        )
                        .Add(T["Divider"], int.MaxValue - 1, divider => divider
                            .Permission(deletePermission)
                            .DividerCss("dropdown-divider").LocalNav()
                        )
                        .Add(reply.IsDeleted ? T["Restore"] : T["Delete"], int.MaxValue, edit => edit
                                .Action(reply.IsDeleted ? "RestoreReply" : "DeleteReply", "Home", "Plato.Ideas",
                                    new RouteValueDictionary()
                                    {
                                        ["id"] = reply.Id
                                    })
                                .Permission(deletePermission)
                                .LocalNav(),
                            reply.IsDeleted
                                ? new List<string>() { "dropdown-item", "dropdown-item-success" }
                                : new List<string>() { "dropdown-item", "dropdown-item-danger" }
                        )
                    , new List<string>() {"idea-comment-options", "text-muted", "dropdown-toggle-no-caret", "text-hidden"}
                );
            
            // If entity & reply are not hidden and entity is not locked allow replies
            if (!entity.IsHidden() && !reply.IsHidden() && !entity.IsLocked)
            {

                builder
                    .Add(T["Comment"], int.MaxValue, options => options
                            .IconCss("fa fa-reply")
                            .Attributes(user == null
                                ? new Dictionary<string, object>()
                                {
                                    {"data-toggle", "tooltip"},
                                    {"title", T["Login to Comment"]}
                                }
                                : new Dictionary<string, object>()
                                {
                                    {"data-provide", "postQuote"},
                                    {"data-quote-selector", "#quote" + reply.Id.ToString()},
                                    {"data-toggle", "tooltip"},
                                    {"title", T["Reply"]}
                                })
                            .Action("Login", "Account", "Plato.Users",
                                new RouteValueDictionary()
                                {
                                    ["returnUrl"] = builder.ActionContext.HttpContext.Request.Path
                                })
                            .Permission(Permissions.PostIdeaComments)
                            .LocalNav()
                        , new List<string>() { "idea-reply", "text-muted", "text-hidden" }
                    );

            }



        }

    }

}

