using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models;

namespace Plato.Labels.Models
{
    public class LabelBase  : IModel<LabelBase>, ILabel
    {

        private readonly ConcurrentDictionary<Type, ISerializable> _metaData;
        
        public int Id { get; set; }

        public int ParentId { get; set; }

        public int FeatureId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Alias { get; set; }

        public string IconCss { get; set; }

        public string ForeColor { get; set; }

        public string BackColor { get; set; }

        public int SortOrder { get; set; }

        public int TotalEntities { get; set; }

        public int TotalFollows { get; set; }

        public int TotalViews { get; set; }

        public int LastEntityId { get; set; }

        public DateTimeOffset? LastEntityDate { get; set; }

        public int CreatedUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }
        
        public int ModifiedUserId { get; set; }
        
        public DateTimeOffset? ModifiedDate { get; set; }
        
        public IEnumerable<LabelData> Data { get; set; } = new List<LabelData>();

        public IDictionary<Type, ISerializable> MetaData => _metaData;
        
        public LabelBase()
        {
            _metaData = new ConcurrentDictionary<Type, ISerializable>();
        }

        public void AddOrUpdate<T>(T obj) where T : class
        {
            if (_metaData.ContainsKey(typeof(T)))
            {
                _metaData.TryUpdate(typeof(T), (ISerializable)obj, _metaData[typeof(T)]);
            }
            else
            {
                _metaData.TryAdd(typeof(T), (ISerializable)obj);
            }
        }

        public void AddOrUpdate(Type type, ISerializable obj)
        {
            if (_metaData.ContainsKey(type))
            {
                _metaData.TryUpdate(type, (ISerializable)obj, _metaData[type]);
            }
            else
            {
                _metaData.TryAdd(type, obj);
            }
        }

        public T GetOrCreate<T>() where T : class
        {
            if (_metaData.ContainsKey(typeof(T)))
            {
                return (T)_metaData[typeof(T)];
            }

            return ActivateInstanceOf<T>.Instance();

        }
        
        public void PopulateModel(IDataReader dr)
        {
            
            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("ParentId"))
                ParentId = Convert.ToInt32(dr["ParentId"]);

            if (dr.ColumnIsNotNull("FeatureId"))
                FeatureId = Convert.ToInt32(dr["FeatureId"]);

            if (dr.ColumnIsNotNull("Name"))
                Name = Convert.ToString(dr["Name"]);

            if (dr.ColumnIsNotNull("Description"))
                Description = Convert.ToString(dr["Description"]);

            if (dr.ColumnIsNotNull("Alias"))
                Alias = Convert.ToString(dr["Alias"]);

            if (dr.ColumnIsNotNull("IconCss"))
                IconCss = Convert.ToString(dr["IconCss"]);

            if (dr.ColumnIsNotNull("ForeColor"))
                ForeColor = Convert.ToString(dr["ForeColor"]);

            if (dr.ColumnIsNotNull("BackColor"))
                BackColor = Convert.ToString(dr["BackColor"]);

            if (dr.ColumnIsNotNull("SortOrder"))
                SortOrder = Convert.ToInt32(dr["SortOrder"]);

            if (dr.ColumnIsNotNull("TotalEntities"))
                TotalEntities = Convert.ToInt32(dr["TotalEntities"]);

            if (dr.ColumnIsNotNull("TotalFollows"))
                TotalEntities = Convert.ToInt32(dr["TotalFollows"]);

            if (dr.ColumnIsNotNull("TotalViews"))
                TotalViews = Convert.ToInt32(dr["TotalViews"]);

            if (dr.ColumnIsNotNull("LastEntityId"))
                LastEntityId = Convert.ToInt32(dr["LastEntityId"]);

            if (dr.ColumnIsNotNull("LastEntityDate"))
                LastEntityDate = DateTimeOffset.Parse(Convert.ToString((dr["LastEntityDate"])));

            if (dr.ColumnIsNotNull("CreatedUserId"))
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = DateTimeOffset.Parse(Convert.ToString((dr["CreatedDate"])));

            if (dr.ColumnIsNotNull("ModifiedUserId"))
                ModifiedUserId = Convert.ToInt32(dr["ModifiedUserId"]);

            if (dr.ColumnIsNotNull("ModifiedDate"))
                ModifiedDate = DateTimeOffset.Parse(Convert.ToString((dr["ModifiedDate"])));

        }

    }

}
