using System;
using System.Collections.Generic;
using System.Text;
using Plato.Badges.Models;

namespace Plato.Users.Badges.ViewModels
{
    public class BadgesIndexViewModel
    {


        public IEnumerable<Badge> Badges { get; set; }

        public BadgesIndexOptions Options { get; set; }

    }

    public class BadgesIndexOptions
    {

        public int UserId { get; set; }

    }

}
