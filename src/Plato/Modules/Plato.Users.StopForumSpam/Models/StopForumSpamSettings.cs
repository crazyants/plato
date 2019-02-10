using System;
using Plato.Internal.Abstractions;

namespace Plato.Users.StopForumSpam.Models
{

    public class StopForumSpamSettings : Serializable
    {

        public string ApiKey { get; set; }
        
    }

    public enum AggressionLevel
    {
        Low, // Forgiving,
        Medium, // Passive
        High // Aggressive
    }

}
