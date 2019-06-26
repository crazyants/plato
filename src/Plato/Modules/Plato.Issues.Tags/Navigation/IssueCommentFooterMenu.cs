using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using Plato.Issues.Models;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Issues.Tags.Navigation
{

    public class IssueCommentFooterMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public IssueCommentFooterMenu(IStringLocalizer localizer)
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

            // We need an entity
            if (entity == null)
            {
                return;
            }

            // Replies are optional
            var reply = builder.ActionContext.HttpContext.Items[typeof(Comment)] as Comment;

            builder
                .Add(T["Tags"], react => react
                    .View("IssueTags", new
                    {
                        entity,
                        reply
                    })
                );

        }

    }

}
