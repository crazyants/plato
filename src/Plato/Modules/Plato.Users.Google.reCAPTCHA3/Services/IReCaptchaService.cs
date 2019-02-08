using Plato.Users.Google.reCAPTCHA3.Models;

namespace Plato.Users.Google.reCAPTCHA3.Services
{

    public delegate void ReCaptchaEventHandler(object sender, ReCaptchaResponse e);

    public interface IReCaptchaService
    {

        event ReCaptchaEventHandler OnComplete;

        void Validate(string encodedResponse);

    }

}
