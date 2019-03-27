using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using Plato.Articles.Models;
using Plato.Entities.Reactions.ViewModels;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Articles.Reactions.Navigation
{
    public class CommentFooterMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }

        public CommentFooterMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }
        
        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "article-comment-footer", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Article)] as Article;
            var reply = builder.ActionContext.HttpContext.Items[typeof(Comment)] as Comment;

            // Add reaction list to navigation
            builder
                .Add(T["React"], int.MaxValue, react => react
                    .View("ReactionList", new
                    {
                        model = new ReactionListViewModel()
                        {
                            Entity = entity,
                            Reply = reply,
                            Permission = Permissions.ReactToComments
                        }
                    })
                    .Permission(Permissions.ViewCommentReactions)
                );

        }

    }

}
