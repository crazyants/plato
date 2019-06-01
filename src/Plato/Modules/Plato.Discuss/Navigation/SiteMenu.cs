using System;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.Navigation
{
    public class SiteMenu : INavigationProvider
    {


        public IStringLocalizer T { get; set; }

        public SiteMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {
            if (!String.Equals(name, "site", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }


            builder
                .Add(T["Discuss"], 1, discuss => discuss
                        .IconCss("fal fa-comment-alt fa-flip-y")
                        .Action("Index", "Home", "Plato.Discuss")
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-provide", "tooltip"},
                            {"title", T["Discuss"]}
                        })
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav(), new List<string>() {"discuss", "text-hidden"}
                );
            
            //builder
            //    .Add(T["Discuss"], 1, discuss => discuss
            //            .IconCss("fal fa-comment-alt fa-flip-y")
            //            .Attributes(new Dictionary<string, object>()
            //            {
            //                {"data-provide", "tooltip"},
            //                {"title", T["Discuss"]}
            //            })
            //            .Add(T["Latest"], int.MinValue, installed => installed
            //                .Action("Index", "Home", "Plato.Discuss")
            //                //.Permission(Permissions.ManageRoles)
            //                .LocalNav()
            //            )
            //            .Add(T["Popular"], int.MinValue + 1, installed => installed
            //                .Action("Popular", "Home", "Plato.Discuss")
            //                //.Permission(Permissions.ManageRoles)
            //                .LocalNav()
            //            ), new List<string>() {"discuss", "text-hidden"}
            //    );

        }
    }

}
