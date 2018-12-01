using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Plato.WebApi.Models;

namespace Plato.Search.Models
{

    [DataContract]
    class SearchApiResult
    {

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "user")]
        public UserApiResult User { get; set; }

        [DataMember(Name = "from")]
        public UserApiResult From { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "date")]
        public IFriendlyDate Date { get; set; }

    }
}
