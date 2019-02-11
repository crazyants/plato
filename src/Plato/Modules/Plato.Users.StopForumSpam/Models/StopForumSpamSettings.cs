using System.Collections.Generic;
using System.Runtime.Serialization;
using Plato.Internal.Abstractions;
using Plato.StopForumSpam.Models;

namespace Plato.Users.StopForumSpam.Models
{

    [DataContract]
    public class StopForumSpamSettings : Serializable
    {
        [DataMember(Name = "apiKey")]
        public string ApiKey { get; set; }

        [DataMember(Name = "spamLevel")]
        public int SpamLevel { get; set; }

        public IEnumerable<SpamOperation> SpamOperations { get; set; }
    
        
    }





    

}
