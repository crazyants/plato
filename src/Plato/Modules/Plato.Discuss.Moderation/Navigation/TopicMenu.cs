using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Moderation.Models;

namespace Plato.Discuss.Moderation.Navigation
{
    public class TopicMenu : INavigationProvider
    {

        private readonly IActionContextAccessor _actionContextAccessor;
    
        public IStringLocalizer T { get; set; }

        public TopicMenu(
            IStringLocalizer localizer,
            IActionContextAccessor actionContextAccessor)
        {
            T = localizer;
            _actionContextAccessor = actionContextAccessor;
        }

        public void BuildNavigation(string name, NavigationBuilder builder)
        {

            if (!String.Equals(name, "topic", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from context
            var topic = builder.ActionContext.HttpContext.Items[typeof(Topic)] as Topic;
            if (topic == null)
            {
                return;
            }
            

            // Get user from context
            var user = builder.ActionContext.HttpContext.Items[typeof(User)] as User;
            if (user == null)
            {
                return;
            }
            
            // Add moderator options
            builder
                .Add(T["Options"], int.MaxValue, options => options
                        .Add(T["Pin Topic"], 1, edit => edit
                            .Action("Edit", "Home", "Plato.Discuss", new RouteValueDictionary()
                            {
                                ["id"] = topic.Id,
                                ["alias"] = topic.Alias
                            })
                            .Permission(ModeratorPermissions.PinTopics)
                            .LocalNav()
                        )
                        .Add(T["Hide Topic"], 2, edit => edit
                                .Action("Edit", "Home", "Plato.Discuss", new RouteValueDictionary()
                                {
                                    ["id"] = topic.Id,
                                    ["alias"] = topic.Alias
                                })
                                .Resource(topic.CategoryId)
                                .Permission(ModeratorPermissions.PinTopics)
                                .LocalNav()
                            ), new List<string>() {"topic-options", "text-muted", "dropdown-toggle-no-caret", "text-hidden"}
                );

        }

    }

}
