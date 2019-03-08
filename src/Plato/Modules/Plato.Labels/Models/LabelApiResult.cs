using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Plato.Labels.Models
{

    [DataContract]
    public class LabelApiResults
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
        public IEnumerable<LabelApiResult> Data { get; set; }

    }

    [DataContract]
    public class LabelApiResult
    {

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "foreColor")]
        public string ForeColor { get; set; }

        [DataMember(Name = "backColor")]
        public string BackColor { get; set; }

        [DataMember(Name = "alias")]
        public string Alias { get; set; }

        [DataMember(Name = "totalEntities")]
        public FriendlyNumber TotalEntities { get; set; }

        [DataMember(Name = "rank")]
        public int Rank { get; set; }

    }

    [DataContract]
    public class FriendlyNumber
    {

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "value")]
        public int Value { get; set; }
        
    }

}
