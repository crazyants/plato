using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using Plato.Ideas.Models;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Ideas.Tags.Navigation
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

            // We need an entity
            if (entity == null)
            {
                return;
            }

            // Replies are optional
            var reply = builder.ActionContext.HttpContext.Items[typeof(IdeaComment)] as IdeaComment;

            builder
                .Add(T["Tags"], react => react
                    .View("IdeaTags", new
                    {
                        entity,
                        reply
                    })
                );

        }

    }

}
