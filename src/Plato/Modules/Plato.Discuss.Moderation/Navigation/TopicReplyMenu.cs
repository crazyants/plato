using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Internal.Navigation;
using Plato.Moderation.Models;

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

            if (!String.Equals(name, "topic-reply", StringComparison.OrdinalIgnoreCase))
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

            // Get moderator from context provided by moderator topic view provider
            var moderator = builder.ActionContext.HttpContext.Items[typeof(Moderator)] as Moderator;

            // We always need a moderator to show this menu
            if (moderator == null)
            {
                return;
            }
      
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
                        .Add(reply.IsPrivate ? T["Publish"] : T["Hide"], 2, edit => edit
                            .Action(reply.IsPrivate ? "ShowReply" : "HideReply", "Home", "Plato.Discuss.Moderation",
                                new RouteValueDictionary()
                                {
                                    ["id"] = reply?.Id ?? 0
                                })
                            .Resource(topic.CategoryId)
                            .Permission(reply.IsPrivate
                                ? ModeratorPermissions.ShowReplies
                                : ModeratorPermissions.HideReplies)
                            .LocalNav()
                        )
                        .Add(reply.IsSpam ? T["Not Spam"] : T["Spam"], 3, spam => spam
                            .Action(reply.IsSpam ? "ReplyFromSpam" : "ReplyToSpam", "Home", "Plato.Discuss.Moderation",
                                new RouteValueDictionary()
                                {
                                    ["id"] = reply?.Id ?? 0
                                })
                            .Resource(topic.CategoryId)
                            .Permission(reply.IsSpam
                                ? ModeratorPermissions.ReplyFromSpam
                                : ModeratorPermissions.ReplyToSpam)
                            .LocalNav()
                        )
                        .Add(T["Divider"], int.MaxValue - 1, divider => divider
                            .Permission(ModeratorPermissions.DeleteReplies)
                            .DividerCss("dropdown-divider").LocalNav()
                        )
                        .Add(reply.IsDeleted ? T["Restore"] : T["Delete"], int.MaxValue, delete => delete
                                .Action(reply.IsDeleted ? "RestoreReply" : "DeleteReply", "Home", "Plato.Discuss.Moderation", new RouteValueDictionary()
                                {
                                    ["id"] = reply?.Id ?? 0
                                })
                                .Resource(topic.CategoryId)
                                .Permission(reply.IsDeleted
                                    ? ModeratorPermissions.RestoreReplies
                                    : ModeratorPermissions.DeleteReplies)
                                .LocalNav(), new List<string>() {"dropdown-item", "dropdown-item-danger"}
                        )

                    , new List<string>() {"topic-options", "text-muted", "dropdown-toggle-no-caret", "text-hidden"}
                );

        }

    }

}