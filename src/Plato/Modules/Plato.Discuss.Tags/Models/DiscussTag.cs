using Plato.Tags.Models;

namespace Plato.Discuss.Tags.Models
{
    public class DiscussTag : Tag
    {
        // A marker class used for discussion view providers

        public DiscussTag()
        {
        }


        public DiscussTag(Tag tag)
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
