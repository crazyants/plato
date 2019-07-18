using System;
using Microsoft.Extensions.Localization;
using Plato.Issues.Models;
using Plato.Entities.Reactions.ViewModels;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Issues.Reactions.Navigation
{
    public class IdeaCommentFooterMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }

        public IdeaCommentFooterMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }
        
        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "issue-comment-footer", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Issue)] as Issue;

            if (entity == null)
            {
                return;
            }

            var reply = builder.ActionContext.HttpContext.Items[typeof(Comment)] as Comment;

            if (reply == null)
            {
                return;
            }
            
            builder
                .Add(T["React"], int.MaxValue, react => react
                    .View("ReactionList", new
                    {
                        model = new ReactionListViewModel()
                        {
                            Entity = entity,
                            Reply = reply,
                            Permission = Permissions.ReactToIssueComments
                        }
                    })
                    .Permission(Permissions.ViewIssueCommentReactions)
                );

        }

    }

}
