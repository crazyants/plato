using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Plato.StopForumSpam.Client.Models
{
    public abstract class Response
    {

        private readonly string _reply;
        private readonly List<ResponsePart> _parts = new List<ResponsePart>();

        protected Response(string reply, string format)
        {
            this._reply = reply;
        }

        protected string GetValue(string key)
        {
            return string.Empty;
        }

        public bool Success { get; internal set; }

        public ResponsePart[] ResponseParts => this._parts.ToArray();

        internal void Add(ResponsePart part)
        {
            this._parts.Add(part);
        }

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

            // We can't guarantee SFS will always return valid JSON.
            // For this reason wrap our parsing within a try catch
            // If parsing fails fallback onto simply looking for the word
            // "success" within the response body. SFS typically confirms submissions
            // with "data submitted successfully" so we'll fall back checking for this

            try
            {

                // Attempt to parse response - this can fail if valid JSON is not supplied
                var obj = JObject.Parse(reply);

                // Parsing succeeded, check results
                if (((int)obj.SelectToken("success")) == 0)
                {

                    // Failed to add spammer

                    return new FailResponse(reply, format)
                    {
                        Error = (string)obj.SelectToken("error"),
                        Success = false
                    };

                }
                else
                {

                    // Spammer added successfully - build response parts

                    var response = new SuccessResponse(reply, format) { Success = true };
                    var email = obj.SelectToken("email");
                    if (email != null)
                    {
                        response.Add(ParseJsonPart(email, RequestType.EmailAddress));
                    }

                    var username = obj.SelectToken("username");
                    if (username != null)
                    {
                        response.Add(ParseJsonPart(username, RequestType.Username));
                    }

                    var ip = obj.SelectToken("ip");
                    if (ip != null)
                    {
                        response.Add(ParseJsonPart(ip, RequestType.IpAddress));
                    }

                    return response;
                }


            }
            catch (Exception e)
            {

                // An error has occurred parsing the response

                // If we find success within the response body return a successful result
                if (reply.IndexOf("success", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return new SuccessResponse(reply, format)
                    {
                        Success = true
                    };
                }

                // If we reach this point return a failure
                return new FailResponse(reply, format)
                {
                    Success = false,
                    Error = e.Message
                };

            }


        }

        private static ResponsePart ParseJsonPart(JToken token, RequestType type)
        {
            var part = new ResponsePart
            {
                Type = type,
                Frequency = (int) token.SelectToken("frequency"),
                Appears = (int) token.SelectToken("appears")
            };

            var lastSeen = token.SelectToken("lastseen");
            if (lastSeen != null)
            {
                if (DateTime.TryParse((string)lastSeen, out var dt))
                {
                    part.LastSeen = dt;
                }
            }

            return part;

        }

    }

}
