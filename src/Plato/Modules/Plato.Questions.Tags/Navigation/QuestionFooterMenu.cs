using System;
using Microsoft.Extensions.Localization;
using Plato.Questions.Models;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Questions.Tags.Navigation
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

            // We always need a topic
            if (entity == null)
            {
                return;
            }

            // Replies are options
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
