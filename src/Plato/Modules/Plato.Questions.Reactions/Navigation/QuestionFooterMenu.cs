using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using Plato.Questions.Models;
using Plato.Entities.Reactions.ViewModels;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Questions.Reactions.Navigation
{
    public class QuestionFooterMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public QuestionFooterMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "question-footer", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Question)] as Question;

            if (entity == null)
            {
                return;
            }

            builder
                .Add(T["Reactions"], int.MaxValue, react => react
                    .View("ReactionList", new
                    {
                        model = new ReactionListViewModel()
                        {
                            Entity = entity,
                            Permission = Permissions.ReactToQuestions
                        }
                    })
                    .Permission(Permissions.ViewQuestionReactions)
                );

        }

    }

}
