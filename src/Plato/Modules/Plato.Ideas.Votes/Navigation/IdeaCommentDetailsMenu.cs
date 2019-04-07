using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;
using Plato.Ideas.Models;
using Plato.Entities.Ratings.ViewModels;

namespace Plato.Ideas.Votes.Navigation
{
    
    public class IdeaCommentDetailsMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }
        private readonly IAuthorizationService _authorizationService;
        public IdeaCommentDetailsMenu(
            IStringLocalizer localizer,
            IAuthorizationService authorizationService)
        {
            T = localizer;
            _authorizationService = authorizationService;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "idea-comment-details", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get entity from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Idea)] as Idea;
            if (entity == null)
            {
                return;
            }

            // Get reply from navigation builder
            var reply = builder.ActionContext.HttpContext.Items[typeof(IdeaComment)] as IdeaComment;
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
