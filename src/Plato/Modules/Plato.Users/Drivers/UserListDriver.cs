using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Plato.Layout.Drivers;
using Plato.Users.ViewModels;

namespace Plato.Users.Drivers
{
    public class UserListDriver : BaseDriver, IViewDriverProvider
    {
        public async Task<IViewDriverResult> Configure()
        { 
            return await Initialize("UserList",
                builder =>
                {
                    builder.UpdateModel(new object());
                    builder.OnDisplay(b => { }); 
                });
        }




    }
}
