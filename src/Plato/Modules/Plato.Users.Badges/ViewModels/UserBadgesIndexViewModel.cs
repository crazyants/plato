using System.Collections.Generic;
using Plato.Internal.Models.Users;
using Plato.Users.ViewModels;

namespace Plato.Users.Badges.ViewModels
{
    
    public class UserBadgesIndexViewModel
    {
    
        public DisplayUserOptions Options { get; set; }

        public User User { get; set; }

        public BadgesIndexViewModel BadgesIndexViewModel { get; set; }
        
    }
    
}
