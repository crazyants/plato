using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using Plato.Docs.Models;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Docs.Tags.Navigation
{

    public class DocCommentFooterMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public DocCommentFooterMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }
        
        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "doc-comment-footer", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Doc)] as Doc;

            // We need an entity
            if (entity == null)
            {
                return;
            }

            // Replies are optional
            var reply = builder.ActionContext.HttpContext.Items[typeof(DocComment)] as DocComment;

            // Add reaction list to topic reply footer navigation
            builder
                .Add(T["Tags"], react => react
                    .View("DocTags", new
                    {
                        entity,
                        reply
                    })
                );

        }

    }

}
