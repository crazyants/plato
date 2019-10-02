using System;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Questions.Models;
using Plato.Entities.History.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Questions.History.Navigation
{
    public class AnswerMenu : INavigationProvider
    {

        private readonly IContextFacade _contextFacade;

        public IStringLocalizer T { get; set; }

        public AnswerMenu(
            IStringLocalizer localizer,
            IContextFacade contextFacade)
        {
            _contextFacade = contextFacade;
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "question-answer", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Question)] as Question;
            var reply = builder.ActionContext.HttpContext.Items[typeof(Answer)] as Answer;

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
                                ["area"] = "Plato.Questions.History",
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
