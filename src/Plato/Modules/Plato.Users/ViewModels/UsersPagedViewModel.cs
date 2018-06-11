using Microsoft.AspNetCore.Html;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Layout.Views;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;

namespace Plato.Users.ViewModels
{

    public class UserViewModel
    {

        public User User { get; set; }

    }

    public class UsersPagedViewModel
    {

        public UsersPagedViewModel()
        {

        }

        public UsersPagedViewModel(
            IPagedResults<User> results,
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {
            this.Results = results;
            this.FilterOpts = filterOptions;
            this.PagerOpts = pagerOptions;
            this.PagerOpts.SetTotal(results.Total);
        }

        public IPagedResults<User> Results { get; set; }

        public PagerOptions PagerOpts { get; set; }

        public FilterOptions FilterOpts { get; set; }
        
        public object PartialView { get; set; }

        public object ViewComponent { get; set; }
    }

    public class FilterOptions
    {
        public string Search { get; set; }

        public UsersOrder Order { get; set; }

    }

    public enum UsersOrder
    {
        Username,
        Email
    }
    
}
