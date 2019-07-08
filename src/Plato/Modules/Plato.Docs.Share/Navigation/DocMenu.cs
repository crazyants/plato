using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Docs.Models;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Docs.Share.Navigation
{
    public class DocMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public DocMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "doc", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            
            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Doc)] as Doc;
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
                            .Action("Index", "Home", "Plato.Docs.Share", new RouteValueDictionary()
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
                            .Permission(Permissions.ShareDocs)
                            .LocalNav()
                        ), new List<string>() {"doc-options", "text-muted", "dropdown-toggle-no-caret", "text-hidden"}
                );

        }

    }

}
