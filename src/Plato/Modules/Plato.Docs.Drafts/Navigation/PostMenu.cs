using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;
using System.Collections.Generic;
using Plato.Docs.Drafts.ViewProviders;
using Plato.Docs.Models;

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

            // Get entity from builder context
            var entity = builder.ActionContext.HttpContext.Items[typeof(Doc)] as Doc;
         
            // Set isHidden flag
            var isHidden = entity?.IsHidden ?? false;
            var isPrivate = entity?.IsPrivate ?? true;
            
            // Ensures we persist selection between post backs
            if (builder.ActionContext.HttpContext.Request.Method == "POST")
            {
                foreach (string key in builder.ActionContext.HttpContext.Request.Form.Keys)
                {
                    if (key.StartsWith(DocViewProvider.HtmlName))
                    {
                        var values = builder.ActionContext.HttpContext.Request.Form[key];
                        foreach (var value in values)
                        {
                            if (value.IndexOf("hidden", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                isHidden = true;
                            }
                            if (value.IndexOf("private", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                isPrivate = true;
                            }
                            if (value.IndexOf("public", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                isHidden = false;
                                isPrivate = false;
                            }
                        }
                    }
                }
            }
            
            // Build navigation
            builder
                .Add(isHidden ? T["Hidden"] : T["Public"], int.MinValue + 10, create => create                                             
                        .View("SelectDropDown", new
                        {
                            model = new SelectDropDownViewModel()
                            {
                                HtmlName = DocViewProvider.HtmlName,
                                SelectedValue = isHidden ? "hidden" : (isPrivate ? "private" : "public"),
                                DropDown = new SelectDropDown()
                                {
                                    Title = "Visibility",
                                    InnerCssClass = "d-block",
                                    Items = new List<SelectDropDownItem>()
                                    {
                                        new SelectDropDownItem()
                                        {
                                            Text = "Private",
                                            Description = "The doc will only be visible to you and those with permission to view private docs. Choose this option whilst your authoring your doc.",
                                            Value = "private",
                                            Permission = Drafts.Permissions.DraftDocToPrivate
                                        },
                                        new SelectDropDownItem()
                                        {
                                            Text = "Ready for Review",
                                            Description = "The doc will be visible to those with permission to view hidden docs. Choose this option once your doc is ready for review.",
                                            Value = "hidden",
                                            Permission = Drafts.Permissions.DraftDocToHidden
                                        },
                                        new SelectDropDownItem()
                                        {
                                            Text = "Public",
                                            Description = "The doc will be visible to everyone. Chose this option once your ready to publish to the world",
                                            Value = "public",
                                            Permission = Drafts.Permissions.DraftDocToPublic
                                        }
                                      
                                    }
                                }
                            }
                        })
                    , new List<string>() {"nav-item", "text-muted"});

        }

    }

}
