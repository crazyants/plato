using System;
using Microsoft.Extensions.Localization;
using Plato.Ideas.Models;
using Plato.Entities.Reactions.ViewModels;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Ideas.Reactions.Navigation
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

            if (!String.Equals(name, "idea-comment-footer", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Idea)] as Idea;

            if (entity == null)
            {
                return;
            }

            var reply = builder.ActionContext.HttpContext.Items[typeof(IdeaComment)] as IdeaComment;

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
                            Permission = Permissions.ReactToIdeaComments
                        }
                    })
                    .Permission(Permissions.ViewIdeaCommentReactions)
                );

        }

    }

}
