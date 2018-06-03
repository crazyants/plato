using Microsoft.AspNetCore.Razor.TagHelpers;
using Plato.Abstractions.Data;
using Plato.Models.Users;
using Plato.Navigation;

namespace Plato.Users.ViewModels
{
    public class UsersPaged
    {
        
        public IPagedResults<User> Results { get; set; }

        public PagerOptions PagerOpts { get; set; }

        public FilterOptions FilterOpts { get; set; }
        
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
