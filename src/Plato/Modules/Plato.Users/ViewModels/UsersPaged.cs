using Plato.Abstractions.Data;
using Plato.Models.Users;

namespace Plato.Users.ViewModels
{
    public class UsersPaged
    {
        
        public IPagedResults<User> PagedResults { get; set; }

        public UsersPagedOptions Options { get; set; }

        public dynamic Pager { get; set; }

    }

    public class UsersPagedOptions
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
