using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Slack.Models
{
    public class SuccessResponse : Response
    {

        public SuccessResponse(string reply, string format) : base(reply, format)
        {
        }

    }

}
