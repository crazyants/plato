using System;
using System.Collections.Generic;
using System.Text;
using Plato.Abstractions.Collections;
using Plato.Models.Users;

namespace Plato.Users.ViewModels
{
    public class UsersPaged
    {
        
        public IPagedResults<User> Users { get; set; }
        //public UserIndexOptions Options { get; set; }
        public dynamic Pager { get; set; }
    }
}
