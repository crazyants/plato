using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Entities.Stores;
using Plato.Internal.Navigation;

namespace Plato.Discuss.History.Navigation
{
    public class TopicMenu : INavigationProvider
    {

        private readonly IEntityStore<Topic> _entityStore;
        private readonly IActionContextAccessor _actionContextAccessor;
    
        public IStringLocalizer T { get; set; }

        public TopicMenu(
            IStringLocalizer localizer,
            IActionContextAccessor actionContextAccessor, IEntityStore<Topic> entityStore)
        {
            T = localizer;
            _actionContextAccessor = actionContextAccessor;
            _entityStore = entityStore;
        }
        
        public void BuildNavigation(string name, NavigationBuilder builder)
        {

            if (!String.Equals(name, "topic", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var topic = builder.ActionContext.HttpContext.Items[typeof(Topic)] as Topic;
            if (topic == null)
            {
                return;
            }
            
            builder
                .Add(T["Edit History"], int.MinValue, history => history
                    .View("HistoryMenu", new
                    {
                        topic = topic
                    })
                );

        }

    }

}
