using Plato.Tags.Models;

namespace Plato.Discuss.Tags.Models
{

    /// <summary>
    /// A marker class used for discussion tag admin view providers
    /// </summary>
    public class TagAdmin : TagBase
    {


        public TagAdmin()
        {
        }
        
        public TagAdmin(TagBase tagBase)
        {
            Id = tagBase.Id;
            FeatureId = tagBase.FeatureId;
            Name = tagBase.Name;
            NameNormalized = tagBase.NameNormalized;
            Description = tagBase.Description;
            Alias = tagBase.Alias;
            TotalEntities = tagBase.TotalEntities;
            TotalFollows = tagBase.TotalFollows;
            LastSeenDate = tagBase.LastSeenDate;
            CreatedUserId = tagBase.CreatedUserId;
            CreatedDate = tagBase.CreatedDate;
        }

    }

}
