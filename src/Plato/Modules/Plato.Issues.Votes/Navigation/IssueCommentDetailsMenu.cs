using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;
using Plato.Issues.Models;
using Plato.Entities.Ratings.ViewModels;

namespace Plato.Issues.Votes.Navigation
{
    
    public class IssueCommentDetailsMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }
        private readonly IAuthorizationService _authorizationService;
        public IssueCommentDetailsMenu(
            IStringLocalizer localizer,
            IAuthorizationService authorizationService)
        {
            T = localizer;
            _authorizationService = authorizationService;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "issue-comment-details", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get entity from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Issue)] as Issue;
            if (entity == null)
            {
                return;
            }

            // Get reply from navigation builder
            var reply = builder.ActionContext.HttpContext.Items[typeof(Comment)] as Comment;
            if (reply == null)
            {
                return;
            }
            
                // Add vote toggle view to navigation
                builder
                .Add(T["Vote"], react => react
                        .View("VoteToggle", new
                        {
                            model = new VoteToggleViewModel()
                            {
                                Entity = entity,
                                Reply = reply,
                                Permission = Permissions.VoteIdeaComments,
                                ApiUrl = "api/ideas/vote/post"
                            }
                        })
                );

        }

    }

}
