using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Issues.Models;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Issues.Share.Navigation
{
    public class IssueMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public IssueMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "issue", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            
            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Issue)] as Issue;
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
                            .Action("Index", "Home", "Plato.Issues.Share", new RouteValueDictionary()
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
                            .Permission(Permissions.ShareIssues)
                            .LocalNav()
                        ), new List<string>() {"issue-options", "text-muted", "dropdown-toggle-no-caret", "text-hidden"}
                );

        }

    }

}
