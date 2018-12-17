using System.Collections.Generic;
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
        
    }

    public class UserIndexOptions
    {
        public string Search { get; set; }

        public bool EnableEdit { get; set; }

        public SortBy Sort { get; set; } = SortBy.LastLoginDate;

        public OrderBy Order { get; set; } = OrderBy.Desc;
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
        Points,
        TotalPoints,
        Follows,
        TotalFollows,
        TotalFollowers,
        Rank,
        CreatedDate,
        LastLoginDate
    }

}
