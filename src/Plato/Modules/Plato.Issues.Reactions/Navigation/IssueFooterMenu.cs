using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using Plato.Issues.Models;
using Plato.Entities.Reactions.ViewModels;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Issues.Reactions.Navigation
{
    public class IssueFooterMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public IssueFooterMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "issue-footer", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Issue)] as Issue;

            if (entity == null)
            {
                return;
            }

            builder
                .Add(T["Reactions"], int.MaxValue, react => react
                    .View("ReactionList", new
                    {
                        model = new ReactionListViewModel()
                        {
                            Entity = entity,
                            Permission = Permissions.ReactToIssues
                        }
                    })
                    .Permission(Permissions.ViewIssueReactions)
                );

        }

    }

}
