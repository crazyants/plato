using System.Threading.Tasks;
using Plato.Users.reCAPTCHA2.Models;

namespace Plato.Users.reCAPTCHA2.Services
{

    public interface IReCaptchaService
    {

        Task<ReCaptchaResponse> Validate(string encodedResponse);

    }

}
