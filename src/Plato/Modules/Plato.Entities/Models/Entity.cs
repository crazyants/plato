using System;
using System.Collections.Generic;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Entities.Models
{
    public class Entity : EntityBase
    {

        private readonly IDictionary<Type, ISerializable> _metaData;

        public IDictionary<Type, ISerializable> MetaData => _metaData;
        
        public Entity()
        {
            // TODO: Replace with concurrent dictionary
            _metaData = new Dictionary<Type, ISerializable>();
        }
        
     
        public void SetMetaData<T>(T obj) where T : class
        {
            _metaData.Add(typeof(T), (ISerializable)obj);
        }

        public void SetMetaData(ISerializable obj, Type type)
        {
            _metaData.Add(type, obj);
        }

        public T GetMetaData<T>() where T : class
        {
            if (_metaData.ContainsKey(typeof(T)))
            {
                return (T)_metaData[typeof(T)];
            }

            return default(T);

        }
        
        public override void PopulateModel(IDataReader dr)
        {
            
            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("FeatureId"))
                FeatureId = Convert.ToInt32(dr["FeatureId"]);

            if (dr.ColumnIsNotNull("Title"))
                Title = Convert.ToString(dr["Title"]);

            if (dr.ColumnIsNotNull("TitleNormalized"))
                TitleNormalized = Convert.ToString(dr["TitleNormalized"]);

            if (dr.ColumnIsNotNull("Markdown"))
                Markdown = Convert.ToString(dr["Markdown"]);
            
            if (dr.ColumnIsNotNull("Html"))
                Html = Convert.ToString(dr["Html"]);

            if (dr.ColumnIsNotNull("PlainText"))
                PlainText = Convert.ToString(dr["PlainText"]);

            if (dr.ColumnIsNotNull("IsPublic"))
                IsPublic = Convert.ToBoolean(dr["IsPublic"]);

            if (dr.ColumnIsNotNull("IsSpam"))
                IsSpam = Convert.ToBoolean(dr["IsSpam"]);

            if (dr.ColumnIsNotNull("IsPinned"))
                IsPinned = Convert.ToBoolean(dr["IsPinned"]);

            if (dr.ColumnIsNotNull("IsDeleted"))
                IsDeleted = Convert.ToBoolean(dr["IsDeleted"]);

            if (dr.ColumnIsNotNull("IsClosed"))
                IsClosed = Convert.ToBoolean(dr["IsClosed"]);

            if (dr.ColumnIsNotNull("CreatedUserId"))
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);

            if (dr.ColumnIsNotNull("ModifiedUserId"))
                ModifiedUserId = Convert.ToInt32(dr["ModifiedUserId"]);

            if (dr.ColumnIsNotNull("ModifiedDate"))
                ModifiedDate = Convert.ToDateTime(dr["ModifiedDate"]);
            
        }

    }

}
