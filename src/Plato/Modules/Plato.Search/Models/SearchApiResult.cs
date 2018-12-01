using System.Runtime.Serialization;
using Plato.WebApi.Models;

namespace Plato.Search.Models
{

    [DataContract]
    public class SearchApiResult
    {

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "createdBy")]
        public UserApiResult CreatedBy { get; set; }

        [DataMember(Name = "modifiedBy")]
        public UserApiResult ModifiedBy { get; set; }

        [DataMember(Name = "lastReplyBy")]
        public UserApiResult LastReplyBy { get; set; }
        
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "excerpt")]
        public string Excerpt { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "createdDate")]
        public IFriendlyDate CreatedDate { get; set; }
        
        [DataMember(Name = "modifiedDate")]
        public IFriendlyDate ModifiedDate { get; set; }

        [DataMember(Name = "lastReplyDate")]
        public IFriendlyDate LastReplyDate { get; set; }

        [DataMember(Name = "relevance")]
        public int Relevance { get; set; }

    }
}
