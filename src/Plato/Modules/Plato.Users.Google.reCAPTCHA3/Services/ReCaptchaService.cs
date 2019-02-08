using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Plato.Users.Google.reCAPTCHA3.Models;

namespace Plato.Users.Google.reCAPTCHA3.Services
{


    public class ReCaptchaService : IReCaptchaService
    {

        public event ReCaptchaEventHandler OnComplete;


        public ReCaptchaService()
        {

        }

        public void Validate(string encodedResponse)
        {

            var privateKey = "6LeY1w4UA44AAAAG3a1f-_0m5-B1jgyCPHVTAmfixj232";
            var uri = new Uri(
                $"https://www.google.com/recaptcha/api/siteverify?secret={privateKey}&response={encodedResponse}");

            var client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DownloadStringCompleted);
            client.DownloadStringAsync(uri);


        }


        void DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            OnComplete?.Invoke(this, JsonConvert.DeserializeObject<ReCaptchaResponse>(e.Result));
        }

    }

}
