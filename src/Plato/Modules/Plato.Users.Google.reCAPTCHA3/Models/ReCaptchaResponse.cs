using Newtonsoft.Json;
using System.Collections.Generic;

namespace Plato.Users.Google.reCAPTCHA3.Models
{
    public class ReCaptchaResponse
    {

        [JsonProperty("success")]
        public string Success { get; set; }

        [JsonProperty("error-codes")]
        public List<string> ErrorCodes { get; set; }

    }
}
