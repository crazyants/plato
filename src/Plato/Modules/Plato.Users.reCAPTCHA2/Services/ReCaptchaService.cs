using System;
using Newtonsoft.Json;
using System.Net;
using Plato.Users.reCAPTCHA2.Models;

namespace Plato.Users.reCAPTCHA2.Services
{


    // https://developers.google.com/recaptcha/docs/display

    public class ReCaptchaService : IReCaptchaService
    {
        
        public ReCaptchaService()
        {

        }

        public ReCaptchaResponse Validate(string encodedResponse)
        {

            var privateKey = "6LeKPZAUAAAAABBL3fkiJD6v6vnSK89TarniqVHm";
            var uri = $"https://www.google.com/recaptcha/api/siteverify?secret={privateKey}&response={encodedResponse}";

            var client = new WebClient();
            var response = client.DownloadString(uri);
            if (!String.IsNullOrEmpty(response))
            {
                return JsonConvert.DeserializeObject<ReCaptchaResponse>(response);
            }

            return null;


        }
        
    }

}
