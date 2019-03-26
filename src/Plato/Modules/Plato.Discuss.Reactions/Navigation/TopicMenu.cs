using System;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Entities.Reactions.ViewModels;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.Reactions.Navigation
{
    public class TopicMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public TopicMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }
        
        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "topic", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Topic)] as Topic;
            if (entity == null)
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
                            ModuleId = "Plato.Discuss.Reactions",
                            Entity = entity
                        }
                    })
                    .Permission(Permissions.ReactToTopics)
                );

        }

    }

}
