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

            // We always need a user for the moderator
            if (user == null)
            {
                return;
            }
            
            // Add moderator options
            builder
                .Add(T["Options"], int.MaxValue, options => options
                        .IconCss("fa fa-ellipsis-h")
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-provide", "tooltip"},
                            {"title", T["Options"]}
                        })
                        .Add(T["Edit"], "1", int.MinValue, edit => edit
                            .Action("Edit", "Home", "Plato.Discuss", new RouteValueDictionary()
                            {
                                ["id"] = topic.Id,
                                ["alias"] = topic.Alias
                            })
                            .Permission(ModeratorPermissions.EditTopics)
                            .LocalNav()
                        )
                        .Add(T["Pin"], 1, edit => edit
                            .Action("Edit", "Home", "Plato.Discuss", new RouteValueDictionary()
                            {
                                ["id"] = topic.Id,
                                ["alias"] = topic.Alias
                            })
                            .Resource(topic.CategoryId)
                            .Permission(ModeratorPermissions.PinTopics)
                            .LocalNav()
                        )
                        .Add(topic.IsClosed ? T["Mark Open"] : T["Mark Closed"], 2, edit => edit
                            .Action(topic.IsClosed ? "OpenTopic" : "CloseTopic", "Home", "Plato.Discuss.Moderation",
                                new RouteValueDictionary()
                                {
                                    ["id"] = topic.Id
                                })
                            .Resource(topic.CategoryId)
                            .Permission(topic.IsClosed
                                ? ModeratorPermissions.OpenTopics
                                : ModeratorPermissions.CloseTopics)
                            .LocalNav()
                        )
                        .Add(topic.IsPrivate ? T["Mark Public"] : T["Mark Private"], 2, edit => edit
                            .Action(topic.IsPrivate ? "ShowTopic" : "HideTopic", "Home", "Plato.Discuss.Moderation",
                                new RouteValueDictionary()
                                {
                                    ["id"] = topic.Id
                                })
                            .Resource(topic.CategoryId)
                            .Permission(topic.IsPrivate
                                ? ModeratorPermissions.ShowTopics
                                : ModeratorPermissions.HideTopics)
                            .LocalNav()
                        )
                        .Add(topic.IsSpam ? T["Remove From Spam"] : T["Mark As Spam"], 2, spam => spam
                            .Action(topic.IsSpam ? "TopicFromSpam" : "TopicToSpam", "Home", "Plato.Discuss.Moderation",
                                new RouteValueDictionary()
                                {
                                    ["id"] = topic.Id
                                })
                            .Resource(topic.CategoryId)
                            .Permission(topic.IsSpam
                                ? ModeratorPermissions.TopicFromSpam
                                : ModeratorPermissions.TopicToSpam)
                            .LocalNav()
                        )
                        .Add(T["Divider"], int.MaxValue - 1, divider => divider
                            .Permission(ModeratorPermissions.DeleteTopics)
                            .DividerCss("dropdown-divider").LocalNav()
                        )
                        .Add(topic.IsDeleted ? T["Restore"] : T["Delete"], int.MaxValue, delete => delete
                                .Action(topic.IsDeleted ? "RestoreTopic" : "DeleteTopic", "Home",
                                    "Plato.Discuss.Moderation", new RouteValueDictionary()
                                    {
                                        ["id"] = topic.Id
                                    })
                                .Resource(topic.CategoryId)
                                .Permission(topic.IsDeleted
                                    ? ModeratorPermissions.RestoreTopics
                                    : ModeratorPermissions.DeleteTopics)
                                .LocalNav(), new List<string>() {"dropdown-item", "dropdown-item-danger"}
                        )
                    , new List<string>() {"topic-options", "text-muted", "dropdown-toggle-no-caret", "text-hidden"}
                );

        }

    }

}
