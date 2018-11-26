using System.Collections.Generic;
using Plato.Internal.Models.Users;

namespace Plato.Discuss.Moderation.ViewModels
{
    public class ModeratorsViewModel
    {

        public IEnumerable<User> Moderators { get; set; }

    }
}
