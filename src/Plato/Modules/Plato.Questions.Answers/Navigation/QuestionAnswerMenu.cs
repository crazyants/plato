using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Entities.Extensions;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Questions.Models;

namespace Plato.Questions.Answers.Navigation
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
            
            // Get authenticated user
            var user = builder.ActionContext.HttpContext.Features[typeof(User)] as User;

            // We need to be authenticated to flag replies as accepted answers
            if (user == null)
            {
                return;
            }

            // Get entity from context
            var entity = builder.ActionContext.HttpContext.Items[typeof(Question)] as Question;
            if (entity == null)
            {
                return;
            }
            
            // Get reply from navigation builder
            var reply = builder.ActionContext.HttpContext.Items[typeof(Answer)] as Answer;
            if (reply == null)
            {
                return;
            }

            // Get permission
            var permission = entity.CreatedUserId == user.Id
                ? Permissions.MarkOwnRepliesAnswer
                : Permissions.MarkAnyReplyAnswer;

            // If entity & reply are not hidden and entity is not locked allow answers
            if (!entity.IsHidden() && !reply.IsHidden() && !entity.IsLocked)
            {

                builder
                    .Add(T["Answer"], int.MinValue, options => options
                            .IconCss(reply.IsAnswer ? "fa fa-times" : "fa fa-check")
                            .Attributes(new Dictionary<string, object>()
                            {
                                {"data-toggle", "tooltip"},
                                {"title", reply.IsAnswer ? T["Remove As Answer"] : T["Mark As Answer"]}
                            })
                            .Action(reply.IsAnswer ? "FromAnswer" : "ToAnswer", "Home", "Plato.Questions.Answers",
                                new RouteValueDictionary()
                                {
                                    ["Id"] = reply.Id
                                })
                            .Permission(permission)
                            .LocalNav()
                        , new List<string>() {"topic-answer", "text-muted", "text-hidden"}
                    );
            }

        }

    }

}
