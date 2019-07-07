using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Articles.Models;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Articles.Share.Navigation
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
            
            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Article)] as Article;
            if (entity == null)
            {
                return;
            }
            
            // Share 
            builder
                .Add(T["Options"], int.MaxValue, options => options
                        .IconCss("fa fa-ellipsis-h")
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-provide", "tooltip"},
                            {"title", T["Options"]}
                        })
                        .Add(T["Share"], int.MaxValue - 3, share => share
                            .Action("Index", "Home", "Plato.Articles.Share", new RouteValueDictionary()
                            {
                                ["opts.id"] = entity.Id.ToString(),
                                ["opts.alias"] = entity.Alias,
                                ["opts.replyId"] = "0"
                            })
                            .Attributes(new Dictionary<string, object>()
                            {
                                {"data-provide", "dialog"},
                                {"data-dialog-modal-css", "modal fade"},
                                {"data-dialog-css", "modal-dialog modal-lg"}
                            })
                            .Permission(Permissions.ShareArticles)
                            .LocalNav()
                        ), new List<string>() {"article-options", "text-muted", "dropdown-toggle-no-caret", "text-hidden"}
                );

        }

    }

}
