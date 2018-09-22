using System;
using System.Collections.Generic;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Models;

namespace Plato.Labels.Models
{

    public interface ILabel : ILabelBase
    {

        int ParentId { get; set; }

        int FeatureId { get; set; }
        
        int SortOrder { get; set; }

        int CreatedUserId { get; set; }

        DateTime? CreatedDate { get; set; }

        int ModifiedUserId { get; set; }

        DateTime? ModifiedDate { get; set; }

        IEnumerable<LabelData> Data { get; set; } 

        IDictionary<Type, ISerializable> MetaData { get; }
        
        void AddOrUpdate<T>(T obj) where T : class;

        void AddOrUpdate(Type type, ISerializable obj);

        T GetOrCreate<T>() where T : class;

        void PopulateModel(IDataReader dr);

    }

}
