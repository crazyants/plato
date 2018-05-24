using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Plato.Abstractions.SetUp;
using Plato.Models.Users;

namespace Plato.Users.Services
{
    public class SetUpEventHandler : ISetUpEventHandler
    {
        private readonly IUserStore<User> _userStoree;
        private readonly UserManager<User> _userManager;

        public SetUpEventHandler(
            UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task SetUp(
            SetUpContext context,
            Action<string, string> reportError
        )
        {


            // create user

            await _userManager.CreateAsync(new User()
            {
                Email = context.AdminEmail,
                UserName = context.AdminUsername
            }, context.AdminPassword);


        }

    }

}
