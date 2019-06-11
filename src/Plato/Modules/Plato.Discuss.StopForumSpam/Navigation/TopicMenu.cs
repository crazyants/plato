using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.StopForumSpam.Navigation
{
    public class TopicMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public TopicMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "topic", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            
            // Get entity from context
            var entity = builder.ActionContext.HttpContext.Items[typeof(Topic)] as Topic;
            if (entity == null)
            {
                return;
            }
            
            // Get reply from context
            var reply = builder.ActionContext.HttpContext.Items[typeof(Reply)] as Reply;
            if (reply == null)
            {
                return;
            }
       
            // If the entity if flagged as spam display additional options
            if (entity.IsSpam)
            {

                builder
                    .Add(T["Add To StopForumSpam"], int.MinValue, options => options
                            .IconCss("fa fa-user-slash")
                            .Attributes(new Dictionary<string, object>()
                            {
                                {"data-toggle", "tooltip"},
                                {"title", T["Help keep the web clean by submit this spammers details to the central StopForumSpam database"]}
                            })
                            .Action("ToStopForumSpam", "Home", "Plato.Questions.Answers",
                                new RouteValueDictionary()
                                {
                                    ["Id"] = reply.Id
                                })
                            .Permission(Permissions.AddToStopForumSpam)
                            .LocalNav()
                        , new List<string>() {"topic-stop-forum-spam", "text-muted", "text-hidden"}
                    );
            }

        }

    }

}
