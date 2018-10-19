using System;
using System.Collections.Generic;
using System.Text;
using Plato.Badges.Models;
using Plato.Internal.Models.Users;

namespace Plato.Users.Badges.ViewModels
{
    public class ProfileDisplayViewModel
    {
        public User User { get; set; }

        public IEnumerable<Badge> Badges { get; set; }

    }
}
