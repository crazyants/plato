using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Localization;
using Plato.Layout.Adaptors;
using Plato.Users.ViewModels;

namespace Plato.Users.Adaptors
{
    public class UserListAdaptor : BaseAdaptor, IViewAdaptorProvider
    {

        public IStringLocalizer T { get; set; }
        
        public UserListAdaptor(IStringLocalizer<AdminMenu> localizer)
        {
            T = localizer;
        }
        
        public override async Task<IViewAdaptorResult> ConfigureAsync()
        {
            return await Adapt("UserList",
                builder =>
                {
                    builder.AdaptView("UserList2");
                    builder.AdaptOutput(output => output);
                    builder.AdaptModel<UsersPaged>(model =>
                    {
                        model.PagerOpts.Page = 10;
                        return model;
                    });
                });

        }

    }
}
