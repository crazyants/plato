using Plato.Follow.Models;

namespace Plato.Follow.ViewModels
{
    public class FollowViewModel
    {

        public int ThingId { get; set; }

        public string FollowHtmlName { get; set; }

        public bool IsFollowing { get; set; }

        public IFollowType FollowType { get; set; }

    }
}
