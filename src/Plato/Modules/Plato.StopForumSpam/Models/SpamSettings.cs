using System.Collections.Generic;
using System.Runtime.Serialization;
using Plato.Internal.Abstractions;

namespace Plato.StopForumSpam.Models
{

    [DataContract]
    public class SpamSettings : Serializable
    {

        [DataMember(Name = "apiKey")]
        public string ApiKey { get; set; }

        [DataMember(Name = "spamLevelId")]
        public int SpamLevelId { get; set; }

        [DataMember(Name = "operations")]
        public IEnumerable<SpamOperation> SpamOperations { get; set; }

    }

}
