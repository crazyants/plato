using Plato.Stars.Models;

namespace Plato.Stars.ViewModels
{
    public class StarViewModel
    {

        public int ThingId { get; set; }

        public string FollowHtmlName { get; set; }

        public bool IsFollowing { get; set; }

        public IStarType FollowType { get; set; }

    }
}
