using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewComponents
{
    
    public class LoginFormViewComponent : ViewComponent
    {

        public LoginFormViewComponent()
        {

        }
        public Task<IViewComponentResult> InvokeAsync(
            string email,
            string userName,
            string password,
            bool rememberMe)
        {
            return Task.FromResult((IViewComponentResult)View(new LoginViewModel()
            {
                Email = email,
                UserName = userName,
                Password = password,
                RememberMe = rememberMe
            }));
        }

    }
}
