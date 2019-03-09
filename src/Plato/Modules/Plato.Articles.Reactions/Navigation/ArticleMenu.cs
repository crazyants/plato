using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using Plato.Articles.Models;
using Plato.Entities.Reactions.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Articles.Reactions.Navigation
{
    public class ArticleMenu : INavigationProvider
    {

        private readonly IEntityStore<Article> _entityStore;
        private readonly IActionContextAccessor _actionContextAccessor;
    
        public IStringLocalizer T { get; set; }

        public ArticleMenu(
            IStringLocalizer localizer,
            IActionContextAccessor actionContextAccessor,
            IEntityStore<Article> entityStore)
        {
            T = localizer;
            _actionContextAccessor = actionContextAccessor;
            _entityStore = entityStore;
        }
        
        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "article", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Article)] as Article;
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
                            ModuleId = "Plato Articles Reactions",
                            Entity = entity
                        }
                    })
                    .Permission(Permissions.ReactToArticles)
                );

        }

    }

}
