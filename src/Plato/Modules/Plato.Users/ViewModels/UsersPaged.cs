using Plato.Abstractions.Data;
using Plato.Models.Users;

namespace Plato.Users.ViewModels
{
    public class UsersPaged
    {
        
        public IPagedResults<User> PagedResults { get; set; }

        public dynamic Pager { get; set; }

    }
}
