using Plato.Stars.Models;

namespace Plato.Stars.ViewModels
{
    public class StarViewModel
    {

        public int ThingId { get; set; }
        
        public bool IsFollowing { get; set; }

        public IStarType StarType { get; set; }

    }
}
