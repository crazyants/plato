using System;
using Microsoft.Extensions.Localization;
using Plato.Issues.Models;
using Plato.Entities.Extensions;
using Plato.Entities.Reactions.ViewModels;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Issues.Reactions.Navigation
{
    public class IssueMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public IssueMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }
        
        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "issue", StringComparison.OrdinalIgnoreCase))
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

            // Don't allow reactions for hidden entities
            if (entity.IsHidden())
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
                            Permission = Permissions.ReactToIssues
                        }
                    })                
                );

        }

    }

}
