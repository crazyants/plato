using System;

namespace Plato.Tags.Models
{
    public interface ITag
    {

        int Id { get; set; }

        int FeatureId { get; set; }
        
        string Name { get; set; }

        string Alias { get; set; }

        int TotalEntities { get; set; }

        int TotalFollows { get; set; }

        int CreatedUserId { get; set; }

        DateTimeOffset? CreatedDate { get; set; }

    }


}
