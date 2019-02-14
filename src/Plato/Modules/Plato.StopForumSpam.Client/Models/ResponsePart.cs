using System;

namespace Plato.StopForumSpam.Client.Models
{

    [Flags]
    public enum RequestType
    {
        IpAddress = 1,
        Username = 2,
        EmailAddress = 4
    }

    public class ResponsePart
    {

        public RequestType Type { get; set; }

        public int Frequency { get; set; }

        public int Appears { get; set; }

        public DateTime LastSeen { get; set; }

    }

}
