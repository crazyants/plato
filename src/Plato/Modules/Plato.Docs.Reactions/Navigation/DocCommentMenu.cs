using System;
using Microsoft.Extensions.Localization;
using Plato.Docs.Models;
using Plato.Entities.Extensions;
using Plato.Entities.Reactions.ViewModels;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Docs.Reactions.Navigation
{
    public class DocCommentMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }

        public DocCommentMenu(IStringLocalizer localizer)
        {
            T = localizer;            
        }
        
        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "doc-comment", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Doc)] as Doc;
            var reply = builder.ActionContext.HttpContext.Items[typeof(DocComment)] as DocComment;
            
            // We need an entity
            if (entity == null)
            {
                return;
            }

            // We need a reply
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
                            ModuleId = "Plato.Docs.Reactions",
                            Entity = entity,
                            Reply = reply,
                            Permission = Permissions.ReactToDocComments
                        }
                    })
                );

        }

    }

}
