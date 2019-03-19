using Plato.Tags.Models;

namespace Plato.Articles.Tags.Models
{

    /// <summary>
    /// A marker class used for discussion tag admin view providers
    /// </summary>
    public class TagAdmin : TagBase
    {
        
        public bool IsNewTag { get; set; }

        public TagAdmin()
        {
        }
        
        public TagAdmin(ITag tag)
        {
            Id = tag.Id;
            FeatureId = tag.FeatureId;
            Name = tag.Name;
            NameNormalized = tag.NameNormalized;
            Description = tag.Description;
            Alias = tag.Alias;
            TotalEntities = tag.TotalEntities;
            TotalFollows = tag.TotalFollows;
            LastSeenDate = tag.LastSeenDate;
            CreatedUserId = tag.CreatedUserId;
            CreatedDate = tag.CreatedDate;
        }

    }

}
