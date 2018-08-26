using System;
using System.Collections.Generic;
using System.Text;
using Plato.Discuss.Moderation.Models;

namespace Plato.Discuss.Moderation.ViewModels
{
    public class ModeratorIndexViewModel
    {

        public IEnumerable<OldModerator> Moderators { get; set; }

    }
}
