using Plato.Users.Google.reCAPTCHA3.Models;

namespace Plato.Users.Google.reCAPTCHA3.Services
{

    public interface IReCaptchaService
    {
        ReCaptchaResponse Validate(string encodedResponse);
    }

}
