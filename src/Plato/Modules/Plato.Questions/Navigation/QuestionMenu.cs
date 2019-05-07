using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Entities.Extensions;
using Plato.Questions.Models;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Questions.Navigation
{
    public class QuestionMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }

        public QuestionMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "question", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get entity from context
            var entity = builder.ActionContext.HttpContext.Items[typeof(Question)] as Question;
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
                    ? Permissions.RestoreOwnQuestions
                    : Permissions.RestoreAnyQuestion;
            }
            else
            {
                // Do we have delete permissions?
                deletePermission = user?.Id == entity.CreatedUserId
                    ? Permissions.DeleteOwnQuestions
                    : Permissions.DeleteAnyAnswer;
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
                            .Action("Edit", "Home", "Plato.Questions", new RouteValueDictionary()
                            {
                                ["opts.id"] = entity.Id,
                                ["opts.alias"] = entity.Alias
                            })
                            .Permission(user?.Id == entity.CreatedUserId
                                ? Permissions.EditOwnQuestions
                                : Permissions.EditAnyQuestion)
                            .LocalNav()
                        )
                           .Add(entity.IsPinned ? T["Unpin"] : T["Pin"], 1, edit => edit
                            .Action(entity.IsPinned ? "Unpin" : "Pin", "Home", "Plato.Questions",
                                new RouteValueDictionary()
                                {
                                    ["id"] = entity.Id
                                })
                            .Resource(entity.CategoryId)
                            .Permission(entity.IsPinned
                                ? Permissions.UnpinQuestions
                                : Permissions.PinQuestions)
                            .LocalNav()
                        )
                        .Add(entity.IsLocked ? T["Unlock"] : T["Lock"], 2, edit => edit
                            .Action(entity.IsLocked ? "Unlock" : "Lock", "Home", "Plato.Questions",
                                new RouteValueDictionary()
                                {
                                    ["id"] = entity.Id
                                })
                            .Resource(entity.CategoryId)
                            .Permission(entity.IsLocked
                                ? Permissions.UnlockQuestions
                                : Permissions.LockQuestions)
                            .LocalNav()
                        )
                        .Add(entity.IsHidden ? T["Unhide"] : T["Hide"], 2, edit => edit
                            .Action(entity.IsHidden ? "Show" : "Hide", "Home", "Plato.Questions",
                                new RouteValueDictionary()
                                {
                                    ["id"] = entity.Id
                                })
                            .Resource(entity.CategoryId)
                            .Permission(entity.IsHidden
                                ? Permissions.ShowQuestions
                                : Permissions.HideQuestions)
                            .LocalNav()
                        )
                        .Add(entity.IsSpam ? T["Not Spam"] : T["Spam"], 2, spam => spam
                            .Action(entity.IsSpam ? "FromSpam" : "ToSpam", "Home", "Plato.Questions",
                                new RouteValueDictionary()
                                {
                                    ["id"] = entity.Id
                                })
                            .Resource(entity.CategoryId)
                            .Permission(entity.IsSpam
                                ? Permissions.QuestionFromSpam
                                : Permissions.QuestionToSpam)
                            .LocalNav()
                        )
                        .Add(T["Report"], int.MaxValue - 2, report => report
                            .Action("Report", "Home", "Plato.Questions", new RouteValueDictionary()
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
                            .Permission(Permissions.ReportQuestions)
                            .LocalNav()
                        )
                        .Add(T["Divider"], int.MaxValue - 1, divider => divider
                            .Permission(deletePermission)
                            .DividerCss("dropdown-divider").LocalNav()
                        )
                        .Add(entity.IsDeleted ? T["Restore"] : T["Delete"], int.MaxValue, edit => edit
                                .Action(entity.IsDeleted ? "Restore" : "Delete", "Home", "Plato.Questions",
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
                    , new List<string>() {"topic-options", "text-muted", "dropdown-toggle-no-caret", "text-hidden"}
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
                                    {"title", T["Reply"]}
                                })
                            .Action("Login", "Account", "Plato.Users",
                                new RouteValueDictionary()
                                {
                                    ["returnUrl"] = builder.ActionContext.HttpContext.Request.Path
                                })
                            .Permission(Permissions.PostAnswers)
                            .LocalNav()
                        , new List<string>() {"topic-reply", "text-muted", "text-hidden"}
                    );

            }

        }

    }

}
