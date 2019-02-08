using Plato.Users.reCAPTCHA2.Models;

namespace Plato.Users.reCAPTCHA2.Services
{

    public interface IReCaptchaService
    {

        ReCaptchaResponse Validate(string encodedResponse);

    }

}
