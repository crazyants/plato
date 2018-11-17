using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Plato.WebApi.Models
{
    
    [DataContract]
    public class PagedApiResults<TModel> : IPagedApiResults<TModel> where TModel : class
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
        public IEnumerable<TModel> Data { get; set; }

    }

}
