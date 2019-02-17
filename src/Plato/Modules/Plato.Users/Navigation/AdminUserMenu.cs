using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;

namespace Plato.Users.Navigation
{
    public class AdminUserMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }
        
        public AdminUserMenu(IStringLocalizer<AdminMenu> localizer)
        {
            T = localizer;
        }
        
        public void BuildNavigation(string name, NavigationBuilder builder)
        {
            if (!String.Equals(name, "admin-user", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get user we are editing from context
            var user = builder.ActionContext.HttpContext.Items[typeof(User)] as User;
            if (user == null)
            {
                return;
            }

            // Add topic options
            builder
                .Add(T["Options"], options => options
                        .IconCss("fa fa-ellipsis-h")
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-provide", "tooltip"},
                            {"title", T["Options"]}
                        })
                        .Add(user.IsVerified ? T["Remove Verified"] : T["Add to Verified"],  edit => edit
                            .Action(user.IsVerified ? "InvalidateUser" : "ValidateUser", "Admin", "Plato.Users", new RouteValueDictionary()
                            {
                                ["Id"] = user.Id.ToString()
                            })
                            .LocalNav()
                        )
                        .Add(T["Divider"], divider => divider
                            .DividerCss("dropdown-divider").LocalNav()
                        )
                        .Add(user.IsSpam ? T["Remove from SPAM"] : T["Add to SPAM"], edit => edit
                            .Action(user.IsSpam ? "RemoveSpam" : "SpamUser", "Admin", "Plato.Users", new RouteValueDictionary()
                            {
                                ["Id"] = user.Id.ToString()
                            })
                            .LocalNav(), user.IsSpam
                                ? new List<string>() { "dropdown-item", "dropdown-item-danger" }
                                : new List<string>() { "dropdown-item" }
                        )
                        .Add(user.IsBanned ? T["Remove Ban"]  : T["Add to Banned"], edit => edit
                            .Action(user.IsBanned  ? "RemoveBan" : "BanUser", "Admin", "Plato.Users", new RouteValueDictionary()
                            {
                                ["Id"] = user.Id.ToString()
                            })
                            .LocalNav(), user.IsBanned
                                ? new List<string>() { "dropdown-item", "dropdown-item-danger" }
                                : new List<string>() { "dropdown-item" }
                        )
                        .Add(T["Divider"], divider => divider
                            .DividerCss("dropdown-divider").LocalNav()
                        )
                        .Add(T["Edit Password"], edit => edit
                            .Action("EditPassword", "Admin", "Plato.Users", new RouteValueDictionary()
                            {
                                ["Id"] = user.Id.ToString()
                            })
                            .LocalNav()
                        )

                    , new List<string>() { "topic-options", "text-muted", "dropdown-toggle-no-caret", "text-hidden" }
                );



        }
    }

}
