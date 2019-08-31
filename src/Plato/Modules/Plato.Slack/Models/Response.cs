using System;
using Newtonsoft.Json.Linq;

namespace Plato.Slack.Models
{
    public class Response
    {

        private readonly string _reply;

        public string Error { get; set; }

        public bool Success { get; internal set; }

        public static Response Parse(string reply, string format)
        {

            if (string.IsNullOrEmpty(reply))
            {
                throw new ArgumentNullException(nameof(reply));
            }

            if (string.IsNullOrEmpty(format))
            {
                throw new ArgumentNullException(nameof(format));
            }

            // Slack returns "ok" if everything was successful
            // If we find "ok" within the response body return a successful result
            if (reply.IndexOf("ok", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return new SuccessResponse(reply, format)
                {
                    Success = true
                };
            }

            // If we reach this point return a failure
            return new FailResponse(reply, format)
            {
                Success = false
            };
            
        }

        protected Response(string reply, string format)
        {
            this._reply = reply;
        }


    }

}
