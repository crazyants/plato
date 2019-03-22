using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using Plato.Entities.Ratings.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Navigation.Abstractions;
using Plato.Questions.Models;

namespace Plato.Questions.Votes.Navigation
{
    public class QuestionDetailsMenu : INavigationProvider
    {

        private readonly IEntityStore<Question> _entityStore;
        private readonly IActionContextAccessor _actionContextAccessor;
    
        public IStringLocalizer T { get; set; }

        public QuestionDetailsMenu(
            IStringLocalizer localizer,
            IActionContextAccessor actionContextAccessor,
            IEntityStore<Question> entityStore)
        {
            T = localizer;
            _actionContextAccessor = actionContextAccessor;
            _entityStore = entityStore;
        }
        
        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "question-details", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Question)] as Question;
            if (entity == null)
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
                            Entity = entity
                        }
                    })
                    //.Permission(Permissions.ReactToTopics)
                );

        }

    }

}
