using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Internal.Navigation;

namespace Plato.Discuss.Tags.Navigation
{
    public class TopicExtraMenu : INavigationProvider
    {

        private readonly IActionContextAccessor _actionContextAccessor;

        public IStringLocalizer T { get; set; }

        public TopicExtraMenu(
            IStringLocalizer localizer,
            IActionContextAccessor actionContextAccessor)
        {
            T = localizer;
            _actionContextAccessor = actionContextAccessor;
        }

        public void BuildNavigation(string name, NavigationBuilder builder)
        {

            if (!String.Equals(name, "topic-extra", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var topic = builder.ActionContext.HttpContext.Items[typeof(Topic)] as Topic;
            var reply = builder.ActionContext.HttpContext.Items[typeof(Reply)] as Reply;
            
            builder
                .Add(T["Tags"], react => react
                    .View("TagList", new
                    {
                        topic,
                        reply
                    })
                );

        }

    }

}
