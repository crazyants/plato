using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;
using Plato.Questions.Models;

namespace Plato.Questions.Answers.Navigation
{
    public class QuestionAnswerDetailsMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public QuestionAnswerDetailsMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }
        
        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "question-answer-details", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Question)] as Question;
            if (entity == null)
            {
                return;
            }

            var reply = builder.ActionContext.HttpContext.Items[typeof(Answer)] as Answer;

            // No reply found on context
            if (reply == null)
            {
                return;
            }

            // Not marked as answer no need to continue
            if (!reply.IsAnswer)
            {
                return;
            }

            // Add reaction menu view to navigation
            builder
                .Add(T["Answer"], int.MinValue, options => options
                        .IconCss("fa fa-check")
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-toggle", "tooltip"},
                            {"title", T["Accepted Answer"]}
                        })
                        .Action("Reply", "Home", "Plato.Questions",
                            new RouteValueDictionary()
                            {
                                ["opts.id"] = entity.Id,
                                ["opts.alias"] = entity.Alias,
                                ["opts.replyId"] = reply.Id
                            })
                        .LocalNav()
                    , new List<string>() { "accepted-answer" }
                );

        }

    }

}
