using System;
using Microsoft.Extensions.Localization;
using Plato.Entities.Ratings.ViewModels;
using Plato.Internal.Navigation.Abstractions;
using Plato.Issues.Models;

namespace Plato.Issues.Votes.Navigation
{
    public class IssueDetailsMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public IssueDetailsMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }
        
        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "issue-details", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Issue)] as Issue;
            if (entity == null)
            {
                return;
            }
       
            // Add reaction menu view to navigation
            builder
                .Add(T["Vote"], react => react
                    .View("VoteToggle", new
                    {
                        model = new VoteToggleViewModel()
                        {                     
                            Entity = entity,
                            Permission = Permissions.VoteIdeas,
                            ApiUrl = "api/ideas/vote/post"
                        }
                    })
                );

        }

    }

}
