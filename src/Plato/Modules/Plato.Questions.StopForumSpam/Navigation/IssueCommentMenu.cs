using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Questions.Models;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Questions.StopForumSpam.Navigation
{
    public class IssueCommentMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public IssueCommentMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "question-answer", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            
            // Get entity from context
            var entity = builder.ActionContext.HttpContext.Items[typeof(Question)] as Question;
            if (entity == null)
            {
                return;
            }

            // Get reply from context
            var reply = builder.ActionContext.HttpContext.Items[typeof(Answer)] as Answer;
            if (reply == null)
            {
                return;
            }

            // If the entity if flagged as spam display additional options
            if (reply.IsSpam)
            {

                builder
                    .Add(T["StopForumSpam"], int.MinValue, options => options
                            .IconCss("fal fa-hand-paper")
                            .Attributes(new Dictionary<string, object>()
                            {
                                {"data-toggle", "tooltip"},
                                {"title", T["Spam Options"]},
                                {"data-provide", "dialog"},
                                {"data-dialog-modal-css", "modal fade"},
                                {"data-dialog-css", "modal-dialog modal-lg"}
                            })
                            .Action("Index", "Home", "Plato.Questions.StopForumSpam",
                                new RouteValueDictionary()
                                {
                                    ["opts.id"] = entity.Id.ToString(),
                                    ["opts.alias"] = entity.Alias,
                                    ["opts.replyId"] = reply.Id.ToString()
                                })
                            .Permission(Permissions.ViewStopForumSpam)
                            .LocalNav()
                        , new List<string>() {"topic-stop-forum-spam", "text-muted", "text-hidden"}
                    );
            }

        }

    }

}
