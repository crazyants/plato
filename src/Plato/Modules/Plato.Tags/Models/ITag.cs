using System;
using Plato.Internal.Abstractions;

namespace Plato.Tags.Models
{
    public interface ITag : IDbModel<TagBase>
    {

        int Id { get; set; }

        int FeatureId { get; set; }
        
        string Name { get; set; }

        string NameNormalized { get; set; }

        string Description { get; set; }

        string Alias { get; set; }

        int TotalEntities { get; set; }

        int TotalFollows { get; set; }

        DateTimeOffset? LastSeenDate { get; set; }

        int CreatedUserId { get; set; }

        DateTimeOffset? CreatedDate { get; set; }
        
    }
    
}
