using System.Runtime.Serialization;
using Plato.WebApi.Models;

namespace Plato.Entities.History.Models
{

    [DataContract]
    class EntityHistoryApiResult
    {

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set;  }
        
        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "createdBy")]
        public UserApiResult CreatedBy { get; set; }

        [DataMember(Name = "date")]
        public IFriendlyDate Date { get; set; }

    }


}
