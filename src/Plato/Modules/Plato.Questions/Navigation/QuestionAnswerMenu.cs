using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Questions.Models;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Questions.Navigation
{

    public class QuestionAnswerMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public QuestionAnswerMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "question-answer", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get topic from context
            var topic = builder.ActionContext.HttpContext.Items[typeof(Question)] as Question;
            if (topic == null)
            {
                return;
            }
            
            // Get reply from context
            var reply = builder.ActionContext.HttpContext.Items[typeof(Answer)] as Answer;
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
                    ? Permissions.RestoreOwnAnswers
                    : Permissions.RestoreAnyAnswer;
            }
            else
            {
                deletePermission = user?.Id == reply.CreatedUserId
                    ? Permissions.DeleteOwnAnswers
                    : Permissions.DeleteAnyAnswer;
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
                            .Action("EditReply", "Home", "Plato.Questions", new RouteValueDictionary()
                            {
                                ["id"] = reply?.Id ?? 0
                            })
                            .Permission(user?.Id == reply.CreatedUserId ?
                                Permissions.EditOwnAnswers :
                                Permissions.EditAnyAnswer)
                            .LocalNav())
                        .Add(T["Report"], int.MaxValue - 2, report => report
                            .Action("Report", "Home", "Plato.Questions", new RouteValueDictionary()
                            {
                                ["opts.id"] = topic.Id,
                                ["opts.alias"] = topic.Alias,
                                ["opts.replyId"] = reply.Id
                            })
                            .Attributes(new Dictionary<string, object>()
                            {
                                {"data-provide", "dialog"},
                                {"data-dialog-modal-css", "modal fade"},
                                {"data-dialog-css", "modal-dialog modal-lg"}
                            })
                            .Permission(Permissions.ReportAnswers)
                            .LocalNav()
                        )
                        .Add(T["Divider"], int.MaxValue - 1, divider => divider
                            .Permission(deletePermission)
                            .DividerCss("dropdown-divider").LocalNav()
                        )
                        .Add(reply.IsDeleted ? T["Restore"] : T["Delete"], int.MaxValue, edit => edit
                                .Action(reply.IsDeleted ? "RestoreReply" : "DeleteReply", "Home", "Plato.Questions",
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
                    , new List<string>() {"topic-options", "text-muted", "dropdown-toggle-no-caret", "text-hidden"}
                );

            if (!topic.IsClosed)
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
                            .Permission(Permissions.PostAnswers)
                            .LocalNav()
                        , new List<string>() { "topic-reply", "text-muted", "text-hidden" }
                    );

            }



        }

    }

}

