using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;
using System.Collections.Generic;
using Plato.Articles.Models;
using Plato.Core.ViewModels;

namespace Plato.Articles.Drafts.Navigation
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
            if (!String.Equals(areaName, "Plato.Articles", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get entity from builder context
            var entity = builder.ActionContext.HttpContext.Items[typeof(Article)] as Article;
         
            // Set isHidden flag
            var isHidden = entity?.IsHidden ?? true;
            
            // Build navigation
            builder
                .Add(isHidden ? T["Hidden"] : T["Public"], int.MinValue + 10, create => create                                             
                        .View("SelectDropDown", new
                        {
                            model = new SelectDropDownViewModel()
                            {
                                SelectedValue = isHidden ? "private" : "public",
                                SelectDropDown = new SelectDropDown()
                                {
                                    Title = "Published?",
                                    InnerCssClass = "d-block",
                                    Items = new List<SelectDropDownItem>()
                                    {
                                        new SelectDropDownItem()
                                        {
                                            Text = "In Draft",
                                            Description = "The article will only be visible to you and those with permission to view private articles. Choose this option whilst your authoring your article.",
                                            Value = "private",
                                        },
                                        new SelectDropDownItem()
                                        {
                                            Text = "Ready for Review",
                                            Description = "The article will be visible to you and those with permission to view hidden articles. Choose this option once your article is ready for peer review.",
                                            Value = "hidden",
                                        },
                                        new SelectDropDownItem()
                                        {
                                            Text = "Published",
                                            Description = "The article will be visible to everyone. Chose this option once your ready to publish to the world",
                                            Value = "public",
                                        }
                                      
                                    }
                                }
                            }
                        })
                    , new List<string>() {"nav-item", "text-muted"});

        }

    }

}
