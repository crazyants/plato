using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;

namespace Plato.Discuss.Moderation.Navigation
{
    public class TopicReplyMenu : INavigationProvider
    {

        private readonly IActionContextAccessor _actionContextAccessor;
    
        public IStringLocalizer T { get; set; }

        public TopicReplyMenu(
            IStringLocalizer localizer,
            IActionContextAccessor actionContextAccessor)
        {
            T = localizer;
            _actionContextAccessor = actionContextAccessor;
        }

        public void BuildNavigation(string name, NavigationBuilder builder)
        {

            if (!String.Equals(name, "topicreply", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var topic = builder.ActionContext.HttpContext.Items[typeof(Topic)] as Topic;
            if (topic == null)
            {
                return;
            }


            // Get model from navigation builder
            var reply = builder.ActionContext.HttpContext.Items[typeof(Reply)] as Reply;
            if (reply == null)
            {
                return;
            }

            //// Get user from context
            var user = builder.ActionContext.HttpContext.Items[typeof(User)] as User;
      
            // Options
            builder
                .Add(T["Options"], int.MaxValue, options => options
                        .IconCss("fa fa-ellipsis-h")
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-provide", "tooltip"},
                            {"title", T["Options"]}
                        })
                        .Add(T["Edit"], "1", int.MinValue, edit => edit
                            .Action("EditReply", "Home", "Plato.Discuss", new RouteValueDictionary()
                            {
                                ["id"] = reply?.Id ?? 0
                            })
                            .Resource(topic.CategoryId)
                            .Permission(ModeratorPermissions.EditReplies)
                            .LocalNav())
                        .Add(T["Hide"], 2, edit => edit
                            .Action("Edit", "Home", "Plato.Discuss", new RouteValueDictionary()
                            {
                                ["id"] = topic.Id,
                                ["alias"] = topic.Alias
                            })
                            .Resource(topic.CategoryId)
                            .Permission(ModeratorPermissions.HideReplies)
                            .LocalNav()
                        )
                        .Add(T["Spam"], 3, spam => spam
                            .Action("Edit", "Home", "Plato.Discuss", new RouteValueDictionary()
                            {
                                ["id"] = topic.Id,
                                ["alias"] = topic.Alias
                            })
                            .Resource(topic.CategoryId)
                            .Permission(ModeratorPermissions.RepliesToSpam)
                            .LocalNav()
                        )
                        .Add(T["Divider"], int.MaxValue - 1, divider => divider
                            .Permission(ModeratorPermissions.DeleteReplies)
                            .DividerCss("dropdown-divider").LocalNav()
                        )
                        .Add(T["Delete"], int.MaxValue, delete => delete
                                .Action("Edit", "Home", "Plato.Discuss", new RouteValueDictionary()
                                {
                                    ["id"] = topic.Id,
                                    ["alias"] = topic.Alias
                                })
                                .Resource(topic.CategoryId)
                                .Permission(ModeratorPermissions.DeleteReplies)
                                .LocalNav(), new List<string>() { "dropdown-item", "dropdown-item-danger" }
                        )

                    , new List<string>() {"topic-options", "text-muted", "dropdown-toggle-no-caret", "text-hidden"}
                );

        }

    }

}