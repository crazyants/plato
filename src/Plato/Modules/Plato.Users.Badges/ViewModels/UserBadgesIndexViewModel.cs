using System.Collections.Generic;
using Plato.Badges.Models;
using Plato.Internal.Models.Users;

namespace Plato.Users.Badges.ViewModels
{
    
    public class UserBadgesIndexViewModel
    {

        public User User { get; set; }

        public BadgesIndexViewModel BadgesIndexViewModel { get; set; }

        public IEnumerable<Badge> Badges { get; set; }
        
    }
}
