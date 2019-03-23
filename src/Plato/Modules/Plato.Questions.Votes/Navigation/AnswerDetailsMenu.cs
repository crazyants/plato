using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;
using Plato.Questions.Models;
using Plato.Entities.Ratings.ViewModels;

namespace Plato.Questions.Votes.Navigation
{
    
    public class AnswerDetailsMenu : INavigationProvider
    {


        public IStringLocalizer T { get; set; }

        public AnswerDetailsMenu(
            IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "answer-details", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get entity from navigation builder
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

            // Add reaction menu view to navigation
            builder
                .Add(T["Vote"], react => react
                        .View("VoteToggle", new
                        {
                            model = new VoteToggleViewModel()
                            {
                                Entity = entity,
                                Reply = reply,
                                ApiUrl = "api/questions/vote/post"
                            }
                        })
                    //.Permission(Permissions.ReactToTopics)
                );

        }

    }

}
