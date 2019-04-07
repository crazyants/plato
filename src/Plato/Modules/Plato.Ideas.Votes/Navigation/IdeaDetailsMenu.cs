using System;
using Microsoft.Extensions.Localization;
using Plato.Entities.Ratings.ViewModels;
using Plato.Internal.Navigation.Abstractions;
using Plato.Ideas.Models;

namespace Plato.Ideas.Votes.Navigation
{
    public class IdeaDetailsMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public IdeaDetailsMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }
        
        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "idea-details", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Idea)] as Idea;
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
