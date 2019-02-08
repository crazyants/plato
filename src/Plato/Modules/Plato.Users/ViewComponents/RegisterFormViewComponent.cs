using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewComponents
{

    public class RegisterFormViewComponent : ViewComponent
    {

        public RegisterFormViewComponent()
        {

        }

        public Task<IViewComponentResult> InvokeAsync(
            string email,
            string userName,
            string password,
            string confirmPassword)
        {
            return Task.FromResult((IViewComponentResult)View(new RegisterViewModel()
            {
                Email = email,
                UserName = userName,
                Password = password,
                ConfirmPassword = confirmPassword
            }));
        }

    }


}
