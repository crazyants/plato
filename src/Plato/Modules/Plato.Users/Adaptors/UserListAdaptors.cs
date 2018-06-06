using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Localization;
using Plato.Layout.Adaptors;

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
                });

        }

    }
}
