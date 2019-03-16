using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.Tags.Navigation
{
    public class TopicFooterMenu : INavigationProvider
    {

        private readonly IActionContextAccessor _actionContextAccessor;

        public IStringLocalizer T { get; set; }

        public TopicFooterMenu(
            IStringLocalizer localizer,
            IActionContextAccessor actionContextAccessor)
        {
            T = localizer;
            _actionContextAccessor = actionContextAccessor;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "topic-footer", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Topic)] as Topic;

            // We always need a topic
            if (entity == null)
            {
                return;
            }

            // Replies are options
            var reply = builder.ActionContext.HttpContext.Items[typeof(Reply)] as Reply;
            
            builder
                .Add(T["Tags"], react => react
                    .View("SimpleTagList", new
                    {
                        entity,
                        reply
                    })
                );

        }

    }

}
