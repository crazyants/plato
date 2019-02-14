using System.Runtime.Serialization;
using Plato.StopForumSpam.Client.Models;

namespace Plato.StopForumSpam.Models
{

    public interface ISpamLevel
    {

        short Id { get; set; }

        string Name { get; set; }

        string Description { get; set; }

        RequestType Flags { get; set; }

    }

    [DataContract]
    public class SpamLevel : ISpamLevel
    {

        [DataMember(Name = "id")]
        public short Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "flags")]
        public RequestType Flags { get; set; }

    }

}
