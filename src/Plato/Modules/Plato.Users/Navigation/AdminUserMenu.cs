using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Users.Navigation
{
    public class AdminUserMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }
        
        public AdminUserMenu(IStringLocalizer<AdminMenu> localizer)
        {
            T = localizer;
        }
        
        public void BuildNavigation(string name, INavigationBuilder builder)
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
                        .Add(user.IsVerified ? T["Remove Verified"] : T["Add to Verified"],  verified => verified
                            .Action(user.IsVerified ? "InvalidateUser" : "ValidateUser", "Admin", "Plato.Users", new RouteValueDictionary()
                            {
                                ["Id"] = user.Id.ToString()
                            })
                            .LocalNav().LocalNav(), new List<string>() { "is-verified" }
                        )
                        .Add(user.IsStaff ? T["Remove Staff"] : T["Add to Staff"], staff => staff
                            .Action(user.IsStaff ? "FromStaff" : "ToStaff", "Admin", "Plato.Users", new RouteValueDictionary()
                            {
                                ["Id"] = user.Id.ToString()
                            })
                            .LocalNav(), new List<string>() { "is-staff"})
                        .Add(T["Divider"], divider => divider
                            .DividerCss("dropdown-divider").LocalNav()
                        )
                        .Add(user.IsSpam ? T["Remove from SPAM"] : T["Add to SPAM"], spam => spam
                            .Action(user.IsSpam ? "RemoveSpam" : "SpamUser", "Admin", "Plato.Users", new RouteValueDictionary()
                            {
                                ["Id"] = user.Id.ToString()
                            })
                            .LocalNav(), user.IsSpam
                                ? new List<string>() { "dropdown-item is-spam", "dropdown-item-danger is-spam" }
                                : new List<string>() { "dropdown-item is-spam" }
                        )
                        .Add(user.IsBanned ? T["Remove Ban"]  : T["Add to Banned"], banned => banned
                            .Action(user.IsBanned  ? "RemoveBan" : "BanUser", "Admin", "Plato.Users", new RouteValueDictionary()
                            {
                                ["Id"] = user.Id.ToString()
                            })
                            .LocalNav(), user.IsBanned
                                ? new List<string>() { "dropdown-item is-banned", "dropdown-item-danger" }
                                : new List<string>() { "dropdown-item is-banned" }
                        )
                        .Add(T["Divider"], divider => divider
                            .DividerCss("dropdown-divider").LocalNav()
                        )
                        .Add(T["Edit Password"], password => password
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
