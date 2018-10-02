using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Plato.WebApi.Models
{
    [DataContract]
    public class UserApiResults
    {

        [DataMember(Name = "page")]
        public int Page { get; set; }

        [DataMember(Name = "size")]
        public int Size { get; set; }

        [DataMember(Name = "total")]
        public int Total { get; set; }

        [DataMember(Name = "totalPages")]
        public int TotalPages { get; set; }

        [DataMember(Name = "data")]
        public IEnumerable<UserApiResult> Data { get; set; }

    }

    [DataContract]
    public class UserApiResult
    {

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }

        [DataMember(Name = "userName")]
        public string UserName { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "rank")]
        public int Rank { get; set; }

    }


}
