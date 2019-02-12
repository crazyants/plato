using System;

namespace Plato.StopForumSpam.Client.Models
{

    public enum RequestType
    {
        IpAddress,
        Username,
        EmailAddress
    }

    public class ResponsePart
    {

        public RequestType Type { get; set; }

        public int Frequency { get; set; }

        public int Appears { get; set; }

        public DateTime LastSeen { get; set; }

    }

}
