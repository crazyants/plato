using System;
using Microsoft.Extensions.Localization;
using Plato.Ideas.Models;
using Plato.Entities.Extensions;
using Plato.Entities.Reactions.ViewModels;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Ideas.Reactions.Navigation
{
    public class IdeaMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public IdeaMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }
        
        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "idea", StringComparison.OrdinalIgnoreCase))
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
                            ModuleId = "Plato.Ideas.Reactions",
                            Entity = entity,
                            Permission = Permissions.ReactToIdeas
                        }
                    })                 
                );

        }

    }

}
