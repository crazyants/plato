using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Plato.Entities.Extensions;
using Plato.Internal.Navigation.Abstractions;
using Plato.Questions.Models;
using Plato.Entities.Ratings.ViewModels;

namespace Plato.Questions.Votes.Navigation
{
    
    public class QuestionAnswerDetailsMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }
        private readonly IAuthorizationService _authorizationService;
        public QuestionAnswerDetailsMenu(
            IStringLocalizer localizer,
            IAuthorizationService authorizationService)
        {
            T = localizer;
            _authorizationService = authorizationService;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "question-answer-details", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get entity from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Question)] as Question;

            // We need an entity
            if (entity == null)
            {
                return;
            }

            // Get reply from navigation builder
            var reply = builder.ActionContext.HttpContext.Items[typeof(Answer)] as Answer;

            // We need a reply
            if (reply == null)
            {
                return;
            }

            // If entity & reply are not hidden allow voting
            if (!entity.IsHidden() && !reply.IsHidden())
            {
                // Add vote toggle view to navigation
                builder
                    .Add(T["Vote"], react => react
                        .View("VoteToggle", new
                        {
                            model = new VoteToggleViewModel()
                            {
                                Entity = entity,
                                Reply = reply,
                                Permission = Permissions.VoteAnswers,
                                ApiUrl = "api/questions/vote/post"
                            }
                        })
                    );
            }

        }

    }

}
