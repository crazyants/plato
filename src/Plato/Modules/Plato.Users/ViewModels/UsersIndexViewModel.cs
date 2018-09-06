using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;

namespace Plato.Users.ViewModels
{

    public class UserViewModel
    {

        public User User { get; set; }

    }

    public class UsersIndexViewModel
    {

        public UsersIndexViewModel()
        {

        }

        public UsersIndexViewModel(
            IPagedResults<User> results,
            ViewOptions viewOptions,
            PagerOptions pagerOptions)
        {
            this.Results = results;
            this.ViewOpts = viewOptions;
            this.PagerOpts = pagerOptions;
            this.PagerOpts.SetTotal(results?.Total ?? 0);
        }

        public IPagedResults<User> Results { get; set; }

        public PagerOptions PagerOpts { get; set; }

        public ViewOptions ViewOpts { get; set; }
    
    }

    public class ViewOptions
    {
        public string Search { get; set; }

        public bool EnableEdit { get; set; }

        public UsersOrder Order { get; set; }

    }

    public enum UsersOrder
    {
        Username,
        Email
    }
    
}
