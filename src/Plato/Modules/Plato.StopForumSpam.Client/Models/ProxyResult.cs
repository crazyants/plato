using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Plato.StopForumSpam.Client.Models
{

    public interface IProxyResult
    {
        bool Appears { get; }

        int Count { get; }

    }

    [DataContract]
    public class ProxyResult : IProxyResult
    {

        public ProxyResult()
        {
        }

        public ProxyResult(int count) : this()
        {
            Count = count;
        }

        [JsonIgnore]
        public bool Appears => Count > 0;

        [DataMember(Name = "count")]
        public int Count { get; }

    }

}
