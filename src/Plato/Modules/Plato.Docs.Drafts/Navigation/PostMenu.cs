using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;
using System.Collections.Generic;
using Plato.Docs.Drafts.ViewModels;

namespace Plato.Docs.Drafts.Navigation
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

            // Ensure we have a dropdown view model provided via our view provider
            if (!(builder.ActionContext.HttpContext.Items[typeof(VisibilityDropDownViewModel)] is VisibilityDropDownViewModel dropdown))
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
            if (!String.Equals(areaName, "Plato.Docs", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Build navigation
            builder
                .Add(dropdown.SelectedValue == "hidden" ? T["Hidden"] : T["Public"], int.MinValue + 10, create => create                                             
                        .View("SelectDropDown", new
                        {
                            model = dropdown

                        })
                    , new List<string>() {"nav-item", "text-muted"});

        }

    }

}
