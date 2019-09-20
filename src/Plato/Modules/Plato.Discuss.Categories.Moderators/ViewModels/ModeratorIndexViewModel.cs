using System.Collections.Generic;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Moderation.Models;

namespace Plato.Discuss.Categories.Moderators.ViewModels
{
    public class ModeratorIndexViewModel
    {

        public PagerOptions PagerOpts { get; set; }

        public FilterOptions FilterOpts { get; set; }

        public IDictionary<SimpleUser, IEnumerable<Moderator>> CategorizedModerators { get; set; }

    }

    public class FilterOptions
    {
        public string Search { get; set; }

    }

}
