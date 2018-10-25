using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Internal.Navigation;

namespace Plato.Discuss.Reactions.Navigation
{
    public class TopicReplyFooterMenu : INavigationProvider
    {

        private readonly IActionContextAccessor _actionContextAccessor;
    
        public IStringLocalizer T { get; set; }

        public TopicReplyFooterMenu(
            IStringLocalizer localizer,
            IActionContextAccessor actionContextAccessor)
        {
            T = localizer;
            _actionContextAccessor = actionContextAccessor;
        }
        
        public void BuildNavigation(string name, NavigationBuilder builder)
        {

            if (!String.Equals(name, "topicreply-footer", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var reply = builder.ActionContext.HttpContext.Items[typeof(Reply)] as Reply;
            
            // Add reaction menu view to navigation
            builder
                .Add(T["React"], react => react
                    .View("TopicReactions", new
                    {
                        id = reply?.Id ?? 0
                    })
                );

            //builder.Add(T["React"], int.MinValue, edit => edit
            //        .IconCss("fal fa-smile")
            //        .Attributes(new Dictionary<string, object>()
            //        {
            //            {"data-provide", "tooltip"},
            //            {"title", T["React to this topic"]}
            //        })
            //        .Action("Edit", "Home", "Plato.Discuss", new RouteValueDictionary()
            //        {
            //            ["id"] = id
            //        })
            //        //.Permission(Permissions.ManageRoles)
            //        .LocalNav()
            //    , new List<string>() {"edit", "text-muted", "text-hidden"});


        }
    }
}
