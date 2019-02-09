using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Plato.StopForumSpam.Models
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
        
            var obj = JObject.Parse(reply);
            if (((int)obj.SelectToken("success")) == 0)
            {
                return new FailResponse(reply, format)
                {
                    Error = (string)obj.SelectToken("error"), Success = false
                };
            }
            else
            {
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
                if (DateTime.TryParse((String)lastSeen, out var dt))
                {
                    part.LastSeen = dt;
                }
            }

            return part;

        }

    }

}
