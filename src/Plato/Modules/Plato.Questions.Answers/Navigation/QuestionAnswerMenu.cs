using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Entities.Ratings.ViewModels;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Questions.Models;

namespace Plato.Questions.Answers.Navigation
{
    public class QuestionAnswerMenu : INavigationProvider
    {


        public IStringLocalizer T { get; set; }

        public QuestionAnswerMenu(
            IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "question-answer", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get entity from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Question)] as Question;
            if (entity == null)
            {
                return;
            }

            // Get authenticated user
            var user = builder.ActionContext.HttpContext.Features[typeof(User)] as User;
            
            // Get reply from navigation builder
            var reply = builder.ActionContext.HttpContext.Items[typeof(Answer)] as Answer;
            if (reply == null)
            {
                return;
            }

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
                        //.Permission(Permissions.PostReplies)
                        .LocalNav()
                    , new List<string>() { "topic-answer", "text-muted", "text-hidden" }
                );

        }

    }

}
