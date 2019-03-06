using Plato.Entities.ViewModels;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.Channels.ViewModels
{
    public class CategoryIndexViewModel 
    {
        
        public PagerOptions Pager { get; set; }

        public CategoryIndexOptions Options { get; set; }

        public EntityIndexOptions EntityIndexOptions { get; set; }
        
    }

    public class CategoryIndexOptions
    {

        public int ChannelId { get; set; }

        public bool EnableEdit { get; set; }

    }
}
