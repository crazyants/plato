using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using Plato.Articles.Models;
using Plato.Entities.Reactions.ViewModels;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Articles.Reactions.Navigation
{
    public class ArticleFooterMenu : INavigationProvider
    {

        private readonly IActionContextAccessor _actionContextAccessor;

        public IStringLocalizer T { get; set; }

        public ArticleFooterMenu(
            IStringLocalizer localizer,
            IActionContextAccessor actionContextAccessor)
        {
            T = localizer;
            _actionContextAccessor = actionContextAccessor;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "article-footer", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Article)] as Article;
            var reply = builder.ActionContext.HttpContext.Items[typeof(Comment)] as Comment;

            // Add reaction list to navigation
            builder
                .Add(T["Reactions"], int.MaxValue, react => react
                    .View("ReactionList", new
                    {
                        model = new ReactionListViewModel()
                        {
                            Entity = entity,
                            Reply = reply
                        }
                    })
                    .Permission(Permissions.ViewArticleReactions)
                );


        }

    }

}
