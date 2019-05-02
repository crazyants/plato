using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using Plato.Questions.Models;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Questions.Tags.Navigation
{

    public class QuestionAnswerFooterMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public QuestionAnswerFooterMenu(IStringLocalizer localizer)
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

            // We need an entity
            if (entity == null)
            {
                return;
            }

            // Replies are optional
            var reply = builder.ActionContext.HttpContext.Items[typeof(Answer)] as Answer;

            builder
                .Add(T["Tags"], react => react
                    .View("QuestionTags", new
                    {
                        entity,
                        reply
                    })
                );

        }

    }

}
