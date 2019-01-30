using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Security.Abstractions;
using Plato.Moderation.Models;
using Plato.Moderation.Stores;

namespace Plato.Discuss.Moderation.Navigation
{
    public class TopicMenu : INavigationProvider
    {
        
        private readonly IModeratorStore<Moderator> _moderatorStore;

        public IStringLocalizer T { get; set; }

        public TopicMenu(
            IStringLocalizer localizer,
            IActionContextAccessor actionContextAccessor,
            IModeratorStore<Moderator> moderatorStore)
        {
            T = localizer;
            _moderatorStore = moderatorStore;
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

            // Get moderator upon first request
            var moderator = builder.ActionContext.HttpContext.Items[typeof(Moderator)] as Moderator;

            // We always need a moderator to render this menu
            if (moderator == null)
            {
                return;
            }
            
            Permission editPermission = null;
            Permission deletePermission = null;
            if (moderator != null)
            {
                // User moderator permissions
                editPermission = ModeratorPermissions.EditTopics;
                deletePermission = ModeratorPermissions.DeleteTopics;
            }
            else
            {
                // User role permissions
                editPermission = user?.Id == topic.CreatedUserId ? Discuss.Permissions.EditOwnTopics : Discuss.Permissions.EditAnyTopic;
                deletePermission = user?.Id == topic.CreatedUserId ? Discuss.Permissions.DeleteOwnTopics : Discuss.Permissions.DeleteAnyTopic;
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
                            .Resource(topic.CategoryId)
                            .Permission(editPermission)
                            .LocalNav()
                        )
                        .Add(topic.IsPinned ? T["Unpin"] : T["Pin"], 1, edit => edit
                            .Action(topic.IsPinned ? "UnpinTopic" : "PinTopic", "Home", "Plato.Discuss.Moderation",
                                new RouteValueDictionary()
                                {
                                    ["id"] = topic.Id
                                })
                            .Resource(topic.CategoryId)
                            .Permission(topic.IsPinned
                                ? ModeratorPermissions.UnpinTopics
                                : ModeratorPermissions.PinTopics)
                            .LocalNav()
                        )
                        .Add(topic.IsClosed ? T["Open"] : T["Close"], 2, edit => edit
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
                        .Add(topic.IsPrivate ? T["Public"] : T["Private"], 2, edit => edit
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
                        .Add(topic.IsSpam ? T["Not Spam"] : T["Spam"], 2, spam => spam
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
                        .Add(topic.IsDeleted ? T["Restore"] : T["Delete"], "1", int.MaxValue, delete => delete
                                .Action(topic.IsDeleted ? "RestoreTopic" : "DeleteTopic", "Home",
                                    "Plato.Discuss.Moderation", new RouteValueDictionary()
                                    {
                                        ["id"] = topic.Id
                                    })
                                .Resource(topic.CategoryId)
                                .Permission(deletePermission)
                                .LocalNav(), new List<string>() {"dropdown-item", "dropdown-item-danger"}
                        )
                    , new List<string>() {"topic-options", "text-muted", "dropdown-toggle-no-caret", "text-hidden"}
                );

        }


        //async Task<Moderator> GetModerator(User user, Topic topic)
        //{


        //    // Get all moderators
        //    var moderators = await _moderatorStore
        //        .QueryAsync()
        //        .ToList();

        //    // No need to check permissions if we don't have any moderators
        //    if (moderators == null)
        //    {
        //        return null;
        //    }

        //    // Get all moderator entries for given identity and resource
        //    var userEntries = moderators.Data
        //        .Where(m => m.UserId == user.Id & m.CategoryId == topic.CategoryId)
        //        .ToList();

        //    // No moderator entries for the user and resource
        //    if (!userEntries.Any())
        //    {
        //        return null;
        //    }

        //    return userEntries[0];

        //}

    }

}
