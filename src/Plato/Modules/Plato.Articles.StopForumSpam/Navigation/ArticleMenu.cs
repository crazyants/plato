using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Articles.Models;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Articles.StopForumSpam.Navigation
{
    public class ArticleMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public ArticleMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "article", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            
            // Get entity from context
            var entity = builder.ActionContext.HttpContext.Items[typeof(Article)] as Article;
            if (entity == null)
            {
                return;
            }
            
            // If the entity if flagged as spam display additional options
            if (entity.IsSpam)
            {

                builder
                    .Add(T["StopForumSpam"], int.MinValue, options => options
                            .IconCss("fal fa-hand-paper")
                            .Attributes(new Dictionary<string, object>()
                            {
                                {"data-toggle", "tooltip"},
                                {"title", T["Spam Options"]},
                                {"data-provide", "dialog"},
                                {"data-dialog-modal-css", "modal fade"},
                                {"data-dialog-css", "modal-dialog modal-lg"}
                            })
                            .Action("Index", "Home", "Plato.Articles.StopForumSpam",
                                new RouteValueDictionary()
                                {
                                    ["opts.id"] = entity.Id.ToString(),
                                    ["opts.alias"] = entity.Alias,
                                    ["opts.replyId"] = "0"
                                })
                            .Permission(Permissions.ViewStopForumSpam)
                            .LocalNav()
                        , new List<string>() {"topic-stop-forum-spam", "text-muted", "text-hidden"}
                    );
            }

        }

    }

}
