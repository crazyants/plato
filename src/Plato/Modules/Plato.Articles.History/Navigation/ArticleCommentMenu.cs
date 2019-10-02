using System;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Articles.Models;
using Plato.Entities.History.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Articles.History.Navigation
{
    public class ArticleCommentMenu : INavigationProvider
    {

        private readonly IContextFacade _contextFacade;

        public IStringLocalizer T { get; set; }

        public ArticleCommentMenu(
            IStringLocalizer localizer,
            IContextFacade contextFacade)
        {
            _contextFacade = contextFacade;
            T = localizer;
        }
        
        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "article-comment", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Article)] as Article;
            var reply = builder.ActionContext.HttpContext.Items[typeof(Comment)] as Comment;

            if (reply == null)
            {
                return;
            }

            // No edited information
            if (reply.EditedDate == null)
            {
                return;
            }

            // Add HistoryMenu view to reply
            builder
                .Add(T["History"], int.MinValue, history => history
                    .View("HistoryMenu", new
                    {
                        model = new HistoryMenuViewModel()
                        {
                            Entity = entity,
                            Reply = reply,
                            DialogUrl = _contextFacade.GetRouteUrl(new RouteValueDictionary()
                            {
                                ["area"] = "Plato.Articles.History",
                                ["controller"] = "Home",
                                ["action"] = "Index",
                                ["id"] = 0
                            })
                        }
                    })
                    .Permission(Permissions.viewReplyHistory)
                );
            
        }

    }

}
