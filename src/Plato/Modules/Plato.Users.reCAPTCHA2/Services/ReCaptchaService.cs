using System;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using Plato.Users.reCAPTCHA2.Models;
using Plato.Users.reCAPTCHA2.Stores;

namespace Plato.Users.reCAPTCHA2.Services
{


    // https://developers.google.com/recaptcha/docs/display

    public class ReCaptchaService : IReCaptchaService
    {
        private readonly IReCaptchaSettingsStore<ReCaptchaSettings> _recaptchaSettingsStore;

        public ReCaptchaService(
            IReCaptchaSettingsStore<ReCaptchaSettings> recaptchaSettingsStore)
        {
            _recaptchaSettingsStore = recaptchaSettingsStore;
        }

        public async Task<ReCaptchaResponse> Validate(string encodedResponse)
        {
            var settings = await _recaptchaSettingsStore.GetAsync();
            var secret = settings?.Secret ?? "";
            var uri = $"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={encodedResponse}";

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
