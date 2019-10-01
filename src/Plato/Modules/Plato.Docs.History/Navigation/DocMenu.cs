using System;
using Microsoft.Extensions.Localization;
using Plato.Docs.Models;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Docs.History.Navigation
{

    public class DocMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public DocMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }
        
        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "doc", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Doc)] as Doc;
            if (entity == null)
            {
                return;
            }

            // Add HistoryMenu view to entity
            builder
                .Add(T["History"], int.MinValue, history => history
                    .View("HistoryMenu", new
                    {
                        entity
                    })
                    .Permission(Permissions.ViewEntityHistory)
                );

        }

    }

}
