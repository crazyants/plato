using Plato.Follows.Models;
using Plato.Internal.Security.Abstractions;

namespace Plato.Follows.ViewModels
{
    public class FollowViewModel
    {

        public int ThingId { get; set; }

        public string FollowHtmlName { get; set; }

        public bool IsFollowing { get; set; }

        public string LoginMessage { get; set; } = "Login to follow";

        public string DenyMessage { get; set; } = "You don't have permission to follow";

        public IFollowType FollowType { get; set; }

        public IPermission Permission { get; set; }
        
    }

}
