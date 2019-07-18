using System;
using Microsoft.Extensions.Localization;
using Plato.Questions.Models;
using Plato.Entities.Reactions.ViewModels;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Questions.Reactions.Navigation
{
    public class AnswerFooterMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }

        public AnswerFooterMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }
        
        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "question-answer-footer", StringComparison.OrdinalIgnoreCase))
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

            if (reply == null)
            {
                return;
            }
            
            builder
                .Add(T["React"], int.MaxValue, react => react
                    .View("ReactionList", new
                    {
                        model = new ReactionListViewModel()
                        {
                            Entity = entity,
                            Reply = reply,
                            Permission = Permissions.ReactToAnswers
                        }
                    })
                    .Permission(Permissions.ViewAnswerReactions)
                );

        }

    }

}
