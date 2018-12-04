using System.Runtime.Serialization;

namespace Plato.Tags.Models
{

    [DataContract]
    public class TagApiResult
    {

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

    }
}
