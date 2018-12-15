using System.Collections.Generic;
using Plato.Internal.Models.Badges;
using Plato.Internal.Models.Users;

namespace Plato.Users.Badges.ViewModels
{
    public class ProfileDisplayViewModel
    {
        public User User { get; set; }

        public IEnumerable<BadgeEntry> Badges { get; set; }

    }
}
