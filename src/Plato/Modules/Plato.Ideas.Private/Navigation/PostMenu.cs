using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;
using System.Collections.Generic;
using Plato.Entities.Private.ViewModels;
using Plato.Ideas.Models;

namespace Plato.Ideas.Private.Navigation
{
    public class PostMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }
        
        public PostMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "post", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get area name
            var areaName = string.Empty;
            if (builder.ActionContext.RouteData.Values.ContainsKey("area"))
            {
                areaName = builder.ActionContext.RouteData.Values["area"].ToString();
            }

            // Ensure we are in the correct area
            if (!String.Equals(areaName, "Plato.Ideas", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get entity from builder context
            var entity = builder.ActionContext.HttpContext.Items[typeof(Idea)] as Idea;
         
            // Set isPrivate flag
            var isPrivate = entity?.IsPrivate ?? false;
            
            // Build navigation
            builder
                .Add(isPrivate ? T["Private"] : T["Public"], int.MinValue + 10, create => create                                             
                        .View("EntityPrivateMenu", new
                        {
                            model = new PrivateMenuViewModel()
                            {
                                IsPrivate = isPrivate
                            }
                        })
                    , new List<string>() {"nav-item", "text-muted"});

        }

    }

}
