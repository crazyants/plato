using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Plato.Users.reCAPTCHA2.Models
{
    public class ReCaptchaResponse
    {

        [JsonProperty("success")]
        public string Success { get; set; }
        
        [JsonProperty("error-codes")]
        public IList<string> ErrorCodes { get; set; } = new List<string>();

        public bool Succeeded
        {
            get
            {
                if (this.Success.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                return false;
            }
        }
        
    }
}
