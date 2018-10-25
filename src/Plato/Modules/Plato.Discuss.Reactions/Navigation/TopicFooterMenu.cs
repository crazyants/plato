using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation;

namespace Plato.Discuss.Reactions.Navigation
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

        public void BuildNavigation(string name, NavigationBuilder builder)
        {

            if (!String.Equals(name, "topic-footer", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get action context
            var context = _actionContextAccessor.ActionContext;

            // Get route values
            var id = context.RouteData.Values["id"].ToString();
            var alias = context.RouteData.Values["alias"].ToString();

            // Ensure we have a valid entity Id
            if (!int.TryParse(id, out var entityId))
            {
                return;
            }
            
            builder
                .Add(T["Reactions"], react => react
                    .View("TopicReactions", new
                    {
                        id = entityId
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
