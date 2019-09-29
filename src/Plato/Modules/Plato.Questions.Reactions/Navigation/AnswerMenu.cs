using System;
using Microsoft.Extensions.Localization;
using Plato.Questions.Models;
using Plato.Entities.Extensions;
using Plato.Entities.Reactions.ViewModels;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Questions.Reactions.Navigation
{
    public class AnswerMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }

        public AnswerMenu(IStringLocalizer localizer)
        {
            T = localizer;            
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "question-answer", StringComparison.OrdinalIgnoreCase))
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

            // No need to show reactions if entity is hidden
            if (entity.IsHidden())
            {
                return;
            }

            // No need to show reactions if reply is hidden
            if (reply.IsHidden())
            {
                return;
            }

            // Add reaction menu view to navigation
            builder
                .Add(T["React"], react => react
                    .View("ReactionMenu", new
                    {
                        model = new ReactionMenuViewModel()
                        {
                            ModuleId = "Plato.Questions.Reactions",
                            Entity = entity,
                            Reply = reply,
                            Permission = Permissions.ReactToAnswers
                        }
                    })
                );

        }

    }

}
