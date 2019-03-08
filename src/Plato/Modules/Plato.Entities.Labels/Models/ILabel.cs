using System;
using System.Collections.Generic;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Models;

namespace Plato.Entities.Labels.Models
{

    public interface ILabel : ILabelBase
    {

        int ParentId { get; set; }

        int FeatureId { get; set; }
        
        int SortOrder { get; set; }

        int TotalEntities { get; set; }

        int TotalFollows { get; set; }

        int TotalViews { get; set; }
        
        int LastEntityId { get; set; }

        DateTimeOffset? LastEntityDate { get; set; }
        
        int CreatedUserId { get; set; }

        DateTimeOffset? CreatedDate { get; set; }

        int ModifiedUserId { get; set; }

        DateTimeOffset? ModifiedDate { get; set; }

        IEnumerable<LabelData> Data { get; set; } 

        IDictionary<Type, ISerializable> MetaData { get; }
        
        void AddOrUpdate<T>(T obj) where T : class;

        void AddOrUpdate(Type type, ISerializable obj);

        T GetOrCreate<T>() where T : class;

        void PopulateModel(IDataReader dr);

    }

}
