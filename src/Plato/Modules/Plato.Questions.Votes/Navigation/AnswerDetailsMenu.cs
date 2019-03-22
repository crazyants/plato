using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using Plato.Entities.Stores;
using Plato.Internal.Navigation.Abstractions;
using Plato.Questions.Models;
using Plato.Entities.Ratings.ViewModels;

namespace Plato.Questions.Votes.Navigation
{
    
    public class AnswerDetailsMenu : INavigationProvider
    {

        private readonly IEntityStore<Question> _entityStore;
        private readonly IActionContextAccessor _actionContextAccessor;

        public IStringLocalizer T { get; set; }

        public AnswerDetailsMenu(
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
                                Reply = reply
                            }
                        })
                    //.Permission(Permissions.ReactToTopics)
                );

        }

    }

}
