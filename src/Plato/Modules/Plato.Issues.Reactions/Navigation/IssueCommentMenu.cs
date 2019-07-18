using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using Plato.Issues.Models;
using Plato.Entities.Extensions;
using Plato.Entities.Reactions.ViewModels;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Issues.Reactions.Navigation
{
    public class IssueCommentMenu : INavigationProvider
    {

        private readonly IActionContextAccessor _actionContextAccessor;

        public IStringLocalizer T { get; set; }

        public IssueCommentMenu(
            IStringLocalizer localizer,
            IActionContextAccessor actionContextAccessor)
        {
            T = localizer;
            _actionContextAccessor = actionContextAccessor;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "issue-comment", StringComparison.OrdinalIgnoreCase))
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
            
            // No need to show reactions if entity is hidden
            if (entity.IsHidden())
            {
                return;
            }

            // No need to show reactions if reply is hidden
            if (reply.IsHidden())
            {
                return;
            }

            // Add reaction menu view to navigation
            builder
                .Add(T["React"], react => react
                    .View("ReactionMenu", new
                    {
                        model = new ReactionMenuViewModel()
                        {
                            ModuleId = "Plato.Issues.Reactions",
                            Entity = entity,
                            Reply = reply
                        }
                    })
                    .Permission(Permissions.ReactToIssueComments)
                );

        }

    }

}
