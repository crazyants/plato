using System;
using Microsoft.Extensions.Localization;
using Plato.Docs.Models;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Docs.History.Navigation
{
    public class DocCommentMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public DocCommentMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }
        
        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "doc-comment", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Doc)] as Doc;
            var reply = builder.ActionContext.HttpContext.Items[typeof(DocComment)] as DocComment;

            // Add HistoryMenu view to reply
            builder
                .Add(T["History"], int.MinValue, history => history
                    .View("HistoryMenu", new
                    {
                        entity,
                        reply
                    })
                    .Permission(Permissions.viewReplyHistory)
                );
            
        }

    }

}
