using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;
using System.Collections.Generic;
using Plato.Ideas.Models;
using Plato.Ideas.Private.ViewProviders;

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

            // Ensures we persist selection between post backs
            if (builder.ActionContext.HttpContext.Request.Method == "POST")
            {
                foreach (string key in builder.ActionContext.HttpContext.Request.Form.Keys)
                {
                    if (key.StartsWith(IdeaViewProvider.HtmlName))
                    {
                        var values = builder.ActionContext.HttpContext.Request.Form[key];
                        foreach (var value in values)
                        {
                            if (value.IndexOf("private", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                isPrivate = true;
                            }
                            if (value.IndexOf("public", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                isPrivate = false;
                            }
                        }
                    }
                }
            }

            // Build navigation
            builder
                .Add(isPrivate ? T["Private"] : T["Public"], int.MinValue + 10, create => create
                        .View("SelectDropDown", new
                        {
                            model = new SelectDropDownViewModel()
                            {
                                HtmlName = IdeaViewProvider.HtmlName,
                                SelectedValue = isPrivate ? "private" : "public",
                                DropDown = new SelectDropDown()
                                {

                                    Title = "Visibility",
                                    InnerCssClass = "d-block",
                                    Items = new List<SelectDropDownItem>()
                                    {
                                        new SelectDropDownItem()
                                        {
                                            Text = "Public",
                                            Description = "This idea will be visible to everyone. Chose this option if your sharing public information and don't mind public comments",
                                            Value = "public"
                                        },
                                        new SelectDropDownItem()
                                        {
                                            Text = "Private",
                                            Description = "This idea will only be visible to you and our team. Choose this option if your sharing private information.",
                                            Value = "private"
                                        }

                                    }
                                }
                            }
                        })
                    , new List<string>() { "nav-item", "text-muted" });

        }

    }

}
