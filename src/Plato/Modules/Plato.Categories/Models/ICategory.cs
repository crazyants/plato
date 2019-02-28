using System;
using System.Collections.Generic;
using System.Data;
using Plato.Internal.Models;

namespace Plato.Categories.Models
{
    public interface ICategory : ICategoryMetaData<CategoryData>, ILabelBase
    {
        
        int ParentId { get; set; }

        int FeatureId { get; set; }

        int SortOrder { get; set; }
        
        int CreatedUserId { get; set; }

        DateTimeOffset? CreatedDate { get; set; }

        int ModifiedUserId { get; set; }

        DateTimeOffset? ModifiedDate { get; set; }
        
        IEnumerable<ICategory> Children { get; set; }

        ICategory Parent { get; set; }

        int Depth { get; set; }

        void PopulateModel(IDataReader dr);

    }
  
}
