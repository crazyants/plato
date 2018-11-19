using System;
using System.Runtime.Serialization;

namespace Plato.WebApi.Models
{
    public interface IFriendlyDate
    {

        string Text { get; set; }

        DateTimeOffset? Value { get; set; }

    }

    [DataContract]
    public class FriendlyDate : IFriendlyDate
    {

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "value")]
        public DateTimeOffset? Value { get; set; }

    }

}
