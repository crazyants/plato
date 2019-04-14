using System;
using Microsoft.Extensions.Localization;
using Plato.Entities.Extensions;
using Plato.Entities.Ratings.ViewModels;
using Plato.Internal.Navigation.Abstractions;
using Plato.Questions.Models;

namespace Plato.Questions.Votes.Navigation
{
    public class QuestionDetailsMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public QuestionDetailsMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }
        
        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "question-details", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Question)] as Question;

            // We always need an entity
            if (entity == null)
            {
                return;
            }

            // If entity is hidden don't allow voting
            if (entity.IsHidden())
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
                            Permission = Permissions.VoteQuestions,
                            ApiUrl = "api/questions/vote/post"
                        }
                    })
                );

        }

    }

}
