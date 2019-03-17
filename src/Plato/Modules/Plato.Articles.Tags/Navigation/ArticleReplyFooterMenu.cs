using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using Plato.Articles.Models;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Articles.Tags.Navigation
{

    public class ArticleReplyFooterMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public ArticleReplyFooterMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }
        
        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "article-reply-footer", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Article)] as Article;

            // We need an entity
            if (entity == null)
            {
                return;
            }

            // Replies are optional
            var reply = builder.ActionContext.HttpContext.Items[typeof(Comment)] as Comment;

            // Add reaction list to topic reply footer navigation
            builder
                .Add(T["Tags"], react => react
                    .View("ArticleTags", new
                    {
                        entity,
                        reply
                    })
                );

        }

    }

}
