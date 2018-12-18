using System.Collections.Generic;
using System.Runtime.Serialization;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;

namespace Plato.Users.ViewModels
{

    public class UserIndexViewModel
    {

        public IPagedResults<User> Results { get; set; }
        
        public UserIndexOptions Options { get; set; }

        public PagerOptions Pager { get; set; }
        
        public IEnumerable<SortColumn> SortColumns { get; set; }

        public IEnumerable<SortOrder> SortOrder { get; set; }

        public IEnumerable<Filter> Filters { get; set; }

    }

    [DataContract]
    public class UserIndexOptions
    {

        [DataMember(Name = "search")]
        public string Search { get; set; }

        [DataMember(Name = "sort")]
        public SortBy Sort { get; set; } = SortBy.LastLoginDate;

        [DataMember(Name = "order")]
        public OrderBy Order { get; set; } = OrderBy.Desc;

        [DataMember(Name = "filter")]
        public FilterBy Filter { get; set; } = FilterBy.All;


        public bool EnableEdit { get; set; }


    }

    public class SortColumn
    {
        public string Text { get; set; }

        public SortBy Value { get; set; }

    }

    public class SortOrder
    {
        public string Text { get; set; }

        public OrderBy Value { get; set; }

    }

    public class Filter
    {
        public string Text { get; set; }

        public FilterBy Value { get; set; }

    }

    public enum FilterBy
    {
        All = 0,
        Confirmed = 1,
        Banned = 2,
        Locked = 3,
        Spam = 4,
        PossibleSpam = 5
    }


    public enum SortBy
    {
        Id,
        UserName,
        Email,
        DisplayName,
        FirstName,
        LastName,
        Visits,
        TotalVisits,
        Minutes,
        MinutesActive,
        Reputation,
        Rank,
        CreatedDate,
        ModifiedDate,
        LastLoginDate
    }

}
