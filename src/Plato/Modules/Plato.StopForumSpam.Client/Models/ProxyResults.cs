using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Plato.StopForumSpam.Client.Models
{

    public interface IProxyResults
    {

        IProxyResult UserName { get; set; }

        IProxyResult Email { get; set; }

        IProxyResult IpAddress { get; set; }

        bool Success { get; set; }

        bool HasResults { get; }

    }

    [DataContract]
    public class ProxyResults : IProxyResults
    {

        [DataMember(Name = "userName")]
        public IProxyResult UserName { get; set; } = new ProxyResult();

        [DataMember(Name = "email")]
        public IProxyResult Email { get; set; } = new ProxyResult();

        [DataMember(Name = "ipAddress")]
        public IProxyResult IpAddress { get; set; } = new ProxyResult();

        [DataMember(Name = "success")]
        public bool Success { get; set; }

        public bool HasResults => UserName.Appears | Email.Appears | IpAddress.Appears;

    }


}
