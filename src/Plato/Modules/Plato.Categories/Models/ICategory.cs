using System;
using System.Collections.Generic;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Models;

namespace Plato.Categories.Models
{
    public interface ICategory : ILabelBase
    {
        
        int ParentId { get; set; }

        int FeatureId { get; set; }

        int SortOrder { get; set; }
        
        int CreatedUserId { get; set; }

        DateTimeOffset? CreatedDate { get; set; }

        int ModifiedUserId { get; set; }

        DateTimeOffset? ModifiedDate { get; set; }

        IEnumerable<CategoryData> Data { get; set; } 

        IDictionary<Type, ISerializable> MetaData { get; }

        IEnumerable<ICategory> Children { get; set; }

        ICategory Parent { get; set; }

        int Depth { get; set; }

        void AddOrUpdate<T>(T obj) where T : class;

        void AddOrUpdate(Type type, ISerializable obj);

        T GetOrCreate<T>() where T : class;

        void PopulateModel(IDataReader dr);

    }

}
