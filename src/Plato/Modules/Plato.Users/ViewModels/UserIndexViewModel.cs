using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;

namespace Plato.Users.ViewModels
{
    
    public class UserIndexViewModel
    {
     
        public IPagedResults<User> Results { get; set; }

        public PagerOptions Pager { get; set; }

        public UserIndexOptions Options { get; set; }

        public UserIndexViewModel()
        {

        }

        public UserIndexViewModel(
            IPagedResults<User> results,
            UserIndexOptions options,
            PagerOptions pager)
        {
            this.Results = results;
            this.Options = options;
            this.Pager = pager;
            this.Pager.SetTotal(results?.Total ?? 0);
        }


    }

    public class UserIndexOptions
    {
        public string Search { get; set; }

        public bool EnableEdit { get; set; }

        public SortBy Sort { get; set; } = SortBy.Email;

        public OrderBy Order { get; set; } = OrderBy.Desc;
    }

    public enum SortBy
    {
        Username,
        Email
    }
    
}
