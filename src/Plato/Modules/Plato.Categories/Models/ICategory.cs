using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Plato.Internal.Abstractions;

namespace Plato.Categories.Models
{
    public interface ICategory
    {

        int Id { get; set; }

        int ParentId { get; set; }

        int FeatureId { get; set; }

        string Name { get; set; }

        string Description { get; set; }

        string Alias { get; set; }

        string IconCss { get; set; }

        string ForeColor { get; set; }

        string BackColor { get; set; }

        int SortOrder { get; set; }

        int CreatedUserId { get; set; }

        DateTime? CreatedDate { get; set; }

        int ModifiedUserId { get; set; }

        DateTime? ModifiedDate { get; set; }

        IEnumerable<CategoryData> Data { get; set; } 

        IDictionary<Type, ISerializable> MetaData { get; }
        

        void AddOrUpdate<T>(T obj) where T : class;

        void AddOrUpdate(Type type, ISerializable obj);

        T GetOrCreate<T>() where T : class;

        void PopulateModel(IDataReader dr);

    }

}
