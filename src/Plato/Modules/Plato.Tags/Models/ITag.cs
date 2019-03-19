using System;
using Plato.Internal.Abstractions;
using Plato.Internal.Models;

namespace Plato.Tags.Models
{
    public interface ITag : ITagBase, IDbModel
    {

        int FeatureId { get; set; }

        string NameNormalized { get; set; }

        int TotalEntities { get; set; }

        int TotalFollows { get; set; }

        DateTimeOffset? LastSeenDate { get; set; }

        int CreatedUserId { get; set; }

        DateTimeOffset? CreatedDate { get; set; }


        int ModifiedUserId { get; set; }

        DateTimeOffset? ModifiedDate { get; set; }

    }

}
