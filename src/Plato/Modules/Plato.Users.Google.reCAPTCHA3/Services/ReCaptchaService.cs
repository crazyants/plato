using Newtonsoft.Json;
using System.Net;
using Plato.Users.Google.reCAPTCHA3.Models;

namespace Plato.Users.Google.reCAPTCHA3.Services
{

    public class ReCaptchaService : IReCaptchaService
    {
        
        public ReCaptchaService()
        {

        }

        public ReCaptchaResponse Validate(string encodedResponse)
        {

            var privateKey = "6LeY1w4UA44AAAAG3a1f-_0m5-B1jgyCPHVTAmfixj232";
            var uri = $"https://www.google.com/recaptcha/api/siteverify?secret={privateKey}&response={encodedResponse}";

            var client = new WebClient();
            var response = client.DownloadString(uri);
            return JsonConvert.DeserializeObject<ReCaptchaResponse>(response);

        }
        
    }

}
