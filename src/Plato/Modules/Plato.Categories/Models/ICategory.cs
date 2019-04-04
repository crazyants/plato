using System;
using Plato.Internal.Abstractions;
using Plato.Internal.Models;

namespace Plato.Categories.Models
{
    public interface ICategory :
        ICategoryMetaData<CategoryData>,
        INestable<ICategory>,
        ILabelBase,
        IDbModel
    {
        
        int ParentId { get; set; }

        int FeatureId { get; set; }
        
        int CreatedUserId { get; set; }

        DateTimeOffset? CreatedDate { get; set; }

        int ModifiedUserId { get; set; }

        DateTimeOffset? ModifiedDate { get; set; }
        
    }
    
}
