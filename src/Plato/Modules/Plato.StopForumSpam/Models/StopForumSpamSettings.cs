using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Plato.Internal.Abstractions;

namespace Plato.StopForumSpam.Models
{

    [DataContract]
    public class StopForumSpamSettings : Serializable
    {
        [DataMember(Name = "apiKey")]
        public string ApiKey { get; set; }

        [DataMember(Name = "spamLevelId")]
        public int SpamLevelId { get; set; }

        [DataMember(Name = "operations")]
        public IEnumerable<SpamOperation> SpamOperations { get; set; }

    }

}
